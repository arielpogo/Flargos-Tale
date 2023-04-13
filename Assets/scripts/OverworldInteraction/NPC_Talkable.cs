using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Talkable : BaseInteractableClass{
    [SerializeField] private DialogueGraph _dialogueGraph;
    [SerializeField] private GameObject _dialogueBoxPrefab;

    public override void Interact() {
        GameObject dialogueBox = Instantiate(_dialogueBoxPrefab, GameManager.Instance.player.transform);
        TMP_Text textBox = dialogueBox.GetComponentInChildren<TMP_Text>();
        Image dialogueBoxPortrait = dialogueBox.transform.Find("DialoguePanel/Portrait").gameObject.GetComponent<Image>();

        StartCoroutine(GameManager.Instance.player.GetComponent<DialogueHandler>().DialogueSequence(_dialogueGraph, textBox, dialogueBoxPortrait, dialogueBox));
    }
}
