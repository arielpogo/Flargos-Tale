using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;
using XNode;

public class DialogueHandler : MonoBehaviour{

    //****************************//
    //                            //
    //         VARIABLES          //
    //                            //
    //****************************//

    //dialogue box prefabs
    [SerializeField] private GameObject[] _dialogueBoxPrefabs = new GameObject[5];

    //dialogue sequencing-related
    private BaseDialogueNode _currentNode;
    private Vector4 _portraitMargins = new(128.0f, 4.0f, 0.0f, 0.0f); //same as below, but margins are moved to the right 128 pixels to make way for the portrait
    private Vector4 _noPortraitMargins = new(0.0f, 4.0f, 0.0f, 0.0f);

    //dialogue writing-related
    const float FONTSIZE = 25.0F;
    const float DELAY = 0.07F; //Default delay, will change with custom markup in the future.

    private bool _currentLineFinished = false;
    private bool _skipText = false; //skip writing, display all text
    private bool _proceed = false; //go to the next dialogue

    private float _delay = DELAY;

    //input-related
    private Vector2 _decisionDirection;

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
    public IEnumerator DialogueSequence(DialogueGraph graph) {
        GameManager.Instance.ChangeGameState(GameState.dialogue);
        _currentNode = graph.GetStartNode();
        GoToNextNodeViaExit("Next"); //go to first node from the start node

        while (_currentNode is not EndNode) {
            Sprite portrait = _currentNode.GetPortrait();

            //spawn the dialogue box
            GameObject dialogueBox = null;
            if (_currentNode is DialogueBoxNode || _currentNode is FloatingDialogueNode) dialogueBox = Instantiate(_dialogueBoxPrefabs[0], GameManager.Instance.player.transform); //0 = default box
            else if (_currentNode is DecisionNode decisionNode) dialogueBox = Instantiate(_dialogueBoxPrefabs[decisionNode.GetDecisions().Length]); //1 = 1 decision; 2 = 2 etc.

            TMP_Text[] textBoxes = null;
            Image dialogueBoxPortrait = null;
            if (dialogueBox) { 
                textBoxes = new TMP_Text[dialogueBox.transform.Find("DialoguePanel/text").childCount];
                //gets the text box components in the children of the text gameobject in the dialogue panel of the dialogue box, in order (message, d1-d4)
                for (int i = 0; i < dialogueBox.transform.Find("DialoguePanel/text").childCount; i++) textBoxes[i] = dialogueBox.transform.Find("DialoguePanel/text").GetChild(i).GetComponent<TMP_Text>();

                dialogueBoxPortrait = dialogueBox.transform.Find("DialoguePanel/Portrait").gameObject.GetComponent<Image>();
            }

            //handle the dialogue box and sequencing
            switch (_currentNode) {
                case DialogueBoxNode _currentNode: {
                        if (portrait) { //if there is a portrait
                            dialogueBoxPortrait.sprite = portrait;
                            dialogueBoxPortrait.color = Color.white;
                            textBoxes[0].margin = _portraitMargins; //textboxes[0] is the main dialogue box
                        }
                        else {
                            textBoxes[0].margin = _noPortraitMargins;
                            dialogueBoxPortrait.color = Color.clear;
                        }

                        textBox.text = "";
                        StartCoroutine(TypeWriter(_currentNode.GetDialogue(), textBoxes[0], _currentNode.GetFontAsset()));
                        yield return new WaitUntil(() => _currentLineFinished);
                        yield return new WaitUntil(() => _proceed); //wait until true
                        _proceed = false;
                        GoToNextNodeViaExit("Next");
                        break;
                    }
                case FloatingDialogueNode _currentNode: { //we don't care if there is an image or not, do not touch the dialogue box
                        dialogueBoxPortrait.sprite = portrait;
                        textBox.text = "";
                        StartCoroutine(TypeWriter(_currentNode.GetDialogue(), textBoxes[0], _currentNode.GetFontAsset()));
                        yield return new WaitUntil(() => _currentLineFinished);
                        yield return new WaitUntil(() => _proceed); //wait until true
                        _proceed = false;
                        GoToNextNodeViaExit("Next");
                        break;
                    }

                case DecisionNode _currentNode:
                    if (portrait) { //if there is a portrait
                        dialogueBoxPortrait.sprite = portrait;
                        dialogueBoxPortrait.color = Color.white;
                        textBoxes[0].margin = _portraitMargins;
                    }
                    else {
                        textBoxes[0].margin = _noPortraitMargins;
                        dialogueBoxPortrait.color = Color.clear;
                    }
                    
                    textBoxes[0].text = "";
                    StartCoroutine(TypeWriter(_currentNode.GetDialogue(), textBoxes[0], fontAsset));
                    yield return new WaitUntil(() => _currentLineFinished);

                    for(int i = 0; i < decisions.Length; i++) {}

                    
                    yield return new WaitUntil(() => _proceed); //wait until true
                    _proceed = false;
                    GoToNextNodeViaExit("Next");
                    break;

                default:
                    throw new NotImplementedException();                    
            }
            if (dialogueBox) Destroy(dialogueBox); //in case we don't want to destroy it, for example the start intro
        }
        _currentNode = null;
        GameManager.Instance.ChangeGameState(GameState.overworld);
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
	
	//Splits into a list of all words, including commands
        string[] words = input.Split(' ');
        string _actualText = String.Empty;
	
	//rebuilds the string without commands
        for(int i = 0; i < words.Length; i++) {
            if (words[i][0] != '|') { //exclude commands
                _actualText += words[i];
                _actualText += " ";
            }
        }
        _actualText = _actualText.TrimEnd(); //removes trailing whitespace
	
	//Sets the text to the text (without commands), but renders 0 of them
        textHolder.text = _actualText;
        textHolder.maxVisibleCharacters = 0;

        char command;
        string commandParameters = string.Empty;

        ///Ok, at this point we have two strings. What was actually inputted into the node (string input) and the modified version without any commands (string _actualText). We're looping through what was actually inputted, "typing" out the characters in _actualText 1:1 with the input. When a command is encountered, the 1:1 is broken, the command is handled (thus i goes ahead of maxVisibleCharacters), then maxInputCharacters continues to go along with the input.
        for (int i = 0; i < input.Length; i++) {
            if (input[i] == '|') {//custom command
                //Debug.Log(input[i]);
                if (i + 1 < input.Length) i++; //if a pipe is the last thing in a string, break
                else break;

                command = input[i];

                //Debug.Log(input[i]);
                if (i + 1 < input.Length) i++; //if the command is the last thing in the string, break
                else break;

                switch (command) {
                    case 'D': //set delay
                        if (_skipText) break; //don't bother changing delay

                        while ((i < input.Length) && Char.IsDigit(input[i])) { //prevent segmentation fault
                            //Debug.Log(input[i]);
                            commandParameters += input[i];
                            i++;
                        }

                        float newDelay; //TryParse sets even if it fails
                        bool success = float.TryParse(commandParameters, out newDelay);

                        if (success) _delay = newDelay / 100.0f;
                        else Debug.Log($"Dialogue Error: Parameter {commandParameters} unable to be parsed for delay command");

                        break;

                    default:
                        Debug.Log($"Dialogue Error: Command {command} unrecognized.");
                        break;
                }
                continue; //skip the space after the command
            }
            if (!_skipText) yield return new WaitForSeconds(_delay);
            if (i < input.Length) textHolder.maxVisibleCharacters++;
        }
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
    public void CutsceneProceed() {
        //if (GameManager.Instance.MasterGameState != GameState.cutscene) Debug.LogWarning("CutsceneProceed called while not in a cutscene.");
        _proceed = true;
    }
    
    //called by Unity events
    public void Decide(InputValue value){
    	_decisionDirection = value.Get<Vector2>();
    }
}
