using UnityEngine;
/// <summary>
/// Used to hold and start the dialogue graph of a cutscene, controlled with a director component.
/// </summary>
public class CutsceneDialogue : MonoBehaviour {
    [SerializeField] private DialogueGraph _dialogueGraph;

    public void CutsceneStart() {
        StartCoroutine(GetComponent<DialogueHandler>().DialogueSequence(_dialogueGraph, gameObject));
    }
}
