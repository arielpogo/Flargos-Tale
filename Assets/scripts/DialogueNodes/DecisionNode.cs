using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using XNode;

/// <summary>
/// Node that presents the player with 4 choices to make
/// </summary>
public class DecisionNode : BaseDialogueNode {
    [Input] public int Previous;
    [Output] public int Outcome1;
    [Output] public int Outcome2;
    [Output] public int Outcome3;
    [Output] public int Outcome4;

    //TODO: Dynamic ports
    public string decision1, decision2, decision3, decision4;
}