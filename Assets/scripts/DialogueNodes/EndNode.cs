using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

/// <summary>
/// Final node in a Dialogue Graph
/// </summary>
[NodeTint("#7C0000")]
public class EndNode : BaseDialogueNode {
    [Input] public int Previous;
}