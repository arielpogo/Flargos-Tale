using UnityEngine;

public class NPCTalkable : BaseInteractableClass{
    [SerializeField] private DialogueGraph _dialogueGraph;
    
    public override void Interact() {
        StartCoroutine(GameManager.Instance.Player.GetComponent<DialogueHandler>().DialogueSequence(_dialogueGraph));
    }
}
