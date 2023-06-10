using TMPro;
using UnityEngine;

public class CutsceneDialogue : MonoBehaviour {
    [SerializeField] private DialogueGraph _dialogueGraph;

    public void CutsceneStart() {
        StartCoroutine(GetComponent<DialogueHandler>().DialogueSequence(_dialogueGraph, gameObject));
    }
}
