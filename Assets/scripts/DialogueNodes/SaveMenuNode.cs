using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// A node that behaves like a DecisionNode, but with preset text, and will call to save the game when Outcome1 is chosen.
/// </summary>
public class SaveMenuNode : BaseDialogueNode {
    [Input] public int Previous;
    [Output] public int Outcome1;
    [Output] public int Outcome2;
    public string dialogue = "Would you like to save?";
    public TMP_FontAsset fontAsset;
    public string[] Decisions = new string[2] {"Yes", "No"};

    public override string GetDialogue() {
        return dialogue;
    }

    public override TMP_FontAsset GetFontAsset() {
        return fontAsset;
    }

    public string[] GetDecisions() {
        return Decisions;
    }
}
