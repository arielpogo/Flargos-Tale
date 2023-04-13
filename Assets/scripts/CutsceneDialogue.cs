using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneDialogue : MonoBehaviour {
    public DialogueGraph dialogueGraph;
    public GameObject textBoxGO;
    public GameObject imageGO;

    public void CutsceneStart() {
        TMP_Text textBox = textBoxGO.GetComponent<TMP_Text>();
        Image image = imageGO.GetComponent<Image>();

        StartCoroutine(GetComponent<DialogueHandler>().DialogueSequence(dialogueGraph, textBox, image, textBoxGO));
    }
}
