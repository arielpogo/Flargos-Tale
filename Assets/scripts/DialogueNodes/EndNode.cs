using UnityEngine;

/// <summary>
/// Final node in a Dialogue Graph
/// </summary>
[NodeTint("#7C0000")]
public class EndNode : BaseDialogueNode {
    [Input] public int Previous;
}