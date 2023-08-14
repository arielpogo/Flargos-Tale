using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using XNode;

public class DialogueHandler : MonoBehaviour {

    //****************************//
    //                            //
    //         VARIABLES          //
    //                            //
    //****************************//
    //private const Dictionary<FontAsset, double> _fontSizeDictionary = new Dictionary<FontAsset, double> (){ };

    //dialogue box prefabs
    [SerializeField] private GameObject[] _dialogueBoxPrefabs = new GameObject[5];

    //dialogue sequencing-related
    private BaseDialogueNode _currentNode;

    //x margin changes for non-decision nodes in boxes with portaits
    private Vector4 _noPortraitMargins = new(18.0f, 14.0f, 18.0f, 14.0f); //move until 18 pixels from the left
    //PortraitXMarginDefault is 140.0f;

    //Y margin changes for decision node decisions if we want extra space
    private Vector4 _decisionLineMarginsExpanded = new(18.0f, 55.0f, 18.0f, 55.0f); //top margins (Y) grows shorter
    //DecisionLineYMarginDefault is 90.0f;

    //W margin changes for decision node messages (top line) if we want extra space
    private Vector4 _decisionMessageMarginsExpanded = new(18.0f, 14.0f, 18.0f, 90.0f); //top message shrinks up
    //DecisionMessageWMarginDefault is 55.0f;

    //dialogue writing-related
    const float FONTSIZE = 30.0F;
    const float DELAY = 0.030F; //Default delay
    private bool _currentLineFinished = false;
    private bool _skipText = false; //skip writing, display all text
    private bool _proceed = false; //go to the next dialogue
    private bool _pauseDialogue = false; //cutscenes can pause dialogue and resume later
    private Vector2 _decisionDirection = Vector2.zero;
    private float _delay = DELAY;
    private Color _colorIdle = Color.white;
    private Color _colorHighlight = new(1, 1, 0, 1);

    //****************************//
    //                            //
    //    DIALOGUE SEQUENCING     //
    //                            //
    //****************************//

    /// <summary>
    /// Begins a dialogue sequence.
    /// </summary>
    /// <param name="graph">The DialogueGraph to read from.</param>
    /// <exception cref="NotImplementedException">Temporarily for the node types I haven't implemented functionality yet.</exception>
    public IEnumerator DialogueSequence(DialogueGraph graph, GameObject DialogueBoxOverride = null) {
        GameEvents.Instance.MajorEvent.Invoke(MajorEvent.dialogue_started);
        _currentNode = graph.GetStartNode();
        GoToNextNodeViaExit("Next"); //go to first node from the start node

        while (_currentNode is not EndNode) {
            if(_pauseDialogue) yield return new WaitUntil(() => !_pauseDialogue); //dialogue paused, e.g. in a cutscene
            Sprite portrait = null;
            portrait = _currentNode.GetPortrait();

            //spawn the dialogue box
            GameObject dialogueBox = DialogueBoxOverride;
            if (_currentNode is DialogueBoxNode) dialogueBox = Instantiate(_dialogueBoxPrefabs[0], GameManager.Instance.Player.transform); //0 = default box
            else if (_currentNode is DecisionNode decisionNode) dialogueBox = Instantiate(_dialogueBoxPrefabs[decisionNode.GetDecisions().Length]); //1 = 1 decision; 2 = 2 etc.
            else if (_currentNode is SaveMenuNode saveMenuNode) dialogueBox = Instantiate(_dialogueBoxPrefabs[saveMenuNode.GetDecisions().Length]);

            TMP_Text[] textBoxes = null;
            Image dialogueBoxPortrait = null;
            if (dialogueBox) {
                textBoxes = new TMP_Text[dialogueBox.transform.Find("text").childCount];
                //gets the text box components in the children of the text gameobject in the dialogue panel of the dialogue box, in order (message, d1-d4)
                for (int i = 0; i < dialogueBox.transform.Find("text").childCount; i++) textBoxes[i] = dialogueBox.transform.Find("text").GetChild(i).GetComponent<TMP_Text>();

                Transform portraitGOTransform = dialogueBox.transform.Find("portrait");
                if (portraitGOTransform) dialogueBoxPortrait = portraitGOTransform.GetComponent<Image>();
            }

            textBoxes[0].text = "";//clear debug message

            if (_currentNode is DialogueBoxNode) {
                if (portrait) { //if there is a portrait
                    dialogueBoxPortrait.sprite = portrait;
                    dialogueBoxPortrait.color = Color.white;
                }
                else {
                    textBoxes[0].margin = _noPortraitMargins;
                    dialogueBoxPortrait.color = Color.clear;
                }
            }
            else if (_currentNode is FloatingDialogueNode && dialogueBoxPortrait) dialogueBoxPortrait.sprite = portrait; //we don't care if there is an image or not, do not touch the dialogue box

            if (_currentNode is DialogueBoxNode || _currentNode is FloatingDialogueNode) {
                StartCoroutine(TypeWriter(_currentNode.GetDialogue(), textBoxes[0], _currentNode.GetFontAsset()));
                yield return new WaitUntil(() => _currentLineFinished);
                yield return new WaitUntil(() => _proceed); //wait until true
                _proceed = false;
                GoToNextNodeViaExit("Next");
            }
            else if (_currentNode is DecisionNode dnode) {
                //decision boxes cannot have portraits
                string[] decisions = dnode.GetDecisions();

                if (dnode.ExtendedSpaceForDecisions) {
                    textBoxes[0].margin = _decisionMessageMarginsExpanded;
                    for (int i = 0; i < decisions.Length; i++) textBoxes[i + 1].margin = _decisionLineMarginsExpanded;
                }

                StartCoroutine(TypeWriter(_currentNode.GetDialogue(), textBoxes[0], _currentNode.GetFontAsset()));
                yield return new WaitUntil(() => _currentLineFinished);

                for (int i = 0; i < decisions.Length; i++) {
                    StartCoroutine(TypeWriter(decisions[i], textBoxes[i + 1], _currentNode.GetFontAsset()));
                    yield return new WaitUntil(() => _currentLineFinished);
                }
                int decision = 0; //0 = not selected, 1-4 is selected, 5 is confirmed
                while (decision != 5) {
                    _decisionDirection = Vector2.zero;
                    yield return new WaitUntil(() => (_decisionDirection != Vector2.zero)); //wait until decision is made
                    textBoxes[decision].color = _colorIdle;
                    switch (decisions.Length) {
                        case 1:
                            if (decision != 1) decision = 1;
                            else { GoToNextNodeViaExit("Outcome1"); decision = 5; }
                            break;
                        case 2:
                            if (_decisionDirection.x < 0 && decision == 1) { GoToNextNodeViaExit("Outcome1"); decision = 5; } //A
                            else if (_decisionDirection.x < 0) decision = 1;
                            else if (decision == 2) { GoToNextNodeViaExit("Outcome2"); decision = 5; }
                            else decision = 2;
                            break;
                        case 3:
                            if (_decisionDirection.x < 0 && decision == 1) { GoToNextNodeViaExit("Outcome1"); decision = 5; } //A
                            else if (_decisionDirection.x < 0) decision = 1;
                            else if (_decisionDirection.x > 0 && decision == 3) { GoToNextNodeViaExit("Outcome3"); decision = 5; } //D
                            else if (_decisionDirection.x > 0) decision = 3;
                            else if (decision == 2) { GoToNextNodeViaExit("Outcome2"); decision = 5; } //W or S
                            else decision = 2;
                            break;
                        case 4:
                            if (_decisionDirection.x < 0 && decision == 1) { GoToNextNodeViaExit("Outcome1"); decision = 5; }//A
                            else if (_decisionDirection.x < 0) decision = 1;
                            else if (_decisionDirection.x > 0 && decision == 4) { GoToNextNodeViaExit("Outcome4"); decision = 5; }//D
                            else if (_decisionDirection.x > 0) decision = 4;
                            else if (_decisionDirection.y > 0 && decision == 2) { GoToNextNodeViaExit("Outcome2"); decision = 5; }//W
                            else if (_decisionDirection.y > 0) decision = 2;
                            else if (decision == 3) { GoToNextNodeViaExit("Outcome3"); decision = 5; }//S
                            else decision = 3;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    if (decision > 0 && decision < 5) textBoxes[decision].color = _colorHighlight;
                }
            }
            else if (_currentNode is SaveMenuNode snode) {
                //decision boxes cannot have portraits
                string[] decisions = snode.GetDecisions();

                StartCoroutine(TypeWriter(_currentNode.GetDialogue(), textBoxes[0], _currentNode.GetFontAsset()));
                yield return new WaitUntil(() => _currentLineFinished);

                for (int i = 0; i < decisions.Length; i++) {
                    StartCoroutine(TypeWriter(decisions[i], textBoxes[i + 1], _currentNode.GetFontAsset()));
                    yield return new WaitUntil(() => _currentLineFinished);
                }

                int decision = 0; //0 = not selected, 1-4 is selected, 5 is confirmed
                while (decision != 5) {
                    _decisionDirection = Vector2.zero;
                    yield return new WaitUntil(() => (_decisionDirection != Vector2.zero)); //wait until decision is made
                    textBoxes[decision].color = _colorIdle;

                    if (_decisionDirection.x < 0 && decision == 1) { GameEvents.Instance.OnDecideSave?.Invoke(); GoToNextNodeViaExit("Outcome1"); decision = 5; }//A
                    else if (_decisionDirection.x < 0) decision = 1;
                    else if (decision == 2) { GoToNextNodeViaExit("Outcome2"); decision = 5; }
                    else decision = 2;

                    if (decision > 0 && decision < 5) textBoxes[decision].color = _colorHighlight;
                }
            }
            else throw new NotImplementedException();

            if (dialogueBox && dialogueBox != DialogueBoxOverride) Destroy(dialogueBox); //in case we don't want to destroy it, for example the start intro
        }
        _currentNode = null;
        GameEvents.Instance.MajorEvent.Invoke(MajorEvent.dialogue_ended);
    }

    /// <summary>
    /// Goes to the next Node via the specified exit point
    /// </summary>
    /// <param name="_exitPoint">The exit point to use</param>
    private void GoToNextNodeViaExit(string _exitPoint) {
        BaseDialogueNode _nextNode = null;

        foreach (NodePort p in _currentNode.Ports) { //loops through the ports
            if (p.fieldName == _exitPoint) {
                _nextNode = p.Connection.node as BaseDialogueNode;
                break;
            }
        }
        if (_nextNode != null) _currentNode = _nextNode;
    }

    //****************************//
    //                            //
    //      DIALOGUE WRITING      //
    //                            //
    //****************************//

    private IEnumerator TypeWriter(string input, TMP_Text textHolder, TMP_FontAsset fontAsset) {
        _currentLineFinished = false;
        _skipText = false;
        _delay = DELAY;

        textHolder.font = fontAsset;
        textHolder.fontSize = FONTSIZE;

        string _actualText = input;
        _actualText = Regex.Replace(_actualText, @"(\|([A-Z]\d*))", String.Empty); //removes commands: \| a pipe [A-Z] a capital letter \d* any number of digits (a command)
        _actualText = _actualText.TrimEnd(); //removes trailing whitespace

        //Sets the text to the text (without commands), but renders 0 of them
        textHolder.text = _actualText;
        textHolder.maxVisibleCharacters = 0;

        ///Ok, at this point we have two strings. What was actually inputted into the node (string input) and the modified version without any commands (string _actualText). We're looping through what was actually inputted, "typing" out the characters in _actualText 1:1 with the input. When a command is encountered, the 1:1 is broken, the command is handled (thus i goes ahead of maxVisibleCharacters), then maxInputCharacters continues to go along with the input.
        for (int i = 0; i < input.Length; i++) {
            if (input[i] == '|') {//custom command
                //Debug.Log(input[i]);
                if (i + 1 < input.Length) i++; //if a pipe is the last thing in a string, break
                else break;

                char command = input[i];
                string commandParameters = string.Empty;

                //Debug.Log(input[i]);
                if (i + 1 < input.Length) i++; //if the command is the last thing in the string, break
                else break;

                if (command == 'D') { //change delay (inverse of speed)
                    if (_skipText) continue;
                    while ((i < input.Length) && Char.IsDigit(input[i])) { //prevent segmentation fault
                        commandParameters += input[i];
                        i++;
                    }
                    i--; //because the loop does i++ again at the end, it'll skip if a new command is right next to this one, so we undo that here

                    float newDelay; //TryParse sets even if it fails
                    bool success = float.TryParse(commandParameters, out newDelay);

                    if (success) _delay = newDelay / 1000.0f;
                    else Debug.Log($"Dialogue Error: Parameter {commandParameters} unable to be parsed for delay command");
                    continue;
                }
                else if (command == 'P') { //pause for a certain number of seconds
                    if (_skipText) continue;
                    while ((i < input.Length) && Char.IsDigit(input[i])) { //prevent segmentation fault
                        commandParameters += input[i];
                        i++;
                    }
                    i--; //because the loop does i++ again at the end, it'll skip if a new command is right next to this one, so we undo that here
                    float wait; //TryParse sets even if it fails
                    bool success = float.TryParse(commandParameters, out wait);
                    if (success) yield return new WaitForSeconds(wait/1000.0f);
                    else Debug.Log($"Dialogue Error: Parameter {commandParameters} unable to be parsed for wait command");
                    continue;
                }
                else Debug.Log($"Dialogue Error: Command {command} unrecognized.");
            }
            if (!_skipText) yield return new WaitForSeconds(_delay);
            //Debug.Log($"Delay: {_delay}, current character: {input[i]}");
            if (i < input.Length) textHolder.maxVisibleCharacters++;
        }
        if (!_skipText) yield return new WaitForSeconds(_delay);
        _currentLineFinished = true;
        //Debug.Log("LINE FINISHED");
    }

    //****************************//
    //                            //
    //           INPUT            //
    //                            //
    //****************************//

    //called by Unity events
    public void OnProceed() {
        if (!_currentLineFinished) _skipText = true;
        else _proceed = true;
    }

    // Only called by cutscene signals. Because they are predictable, we do not need to check them.
    public void CutsceneProceed() => _proceed = true;

    public void CutsceneTogglePause() {
        if (_pauseDialogue) _pauseDialogue = false;
        else _pauseDialogue = true;
    }

    //called by unity input events
    public void OnDecide(InputValue value) {
        _decisionDirection = value.Get<Vector2>();
        _decisionDirection.x = Math.Sign(_decisionDirection.x); //unit vector
        _decisionDirection.y = Math.Sign(_decisionDirection.y);
    }
}