using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

/// <summary>
/// First node in a Dialogue Graph
/// </summary>
[NodeTint("#002F00")]
public class StartNode : BaseDialogueNode {
	[Output] public int Next;
}