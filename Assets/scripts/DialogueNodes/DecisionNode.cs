using TMPro;
using UnityEngine;

/// <summary>
/// Node that presents the player with 4 choices to make
/// </summary>
public class DecisionNode : BaseDialogueNode {
    [Input] public int Previous;
    [Output] public int Outcome1;
    [Output] public int Outcome2;
    [Output] public int Outcome3;
    [Output] public int Outcome4;
    public string dialogue;
    public TMP_FontAsset fontAsset;
    public string[] Decisions = new string[4];
    public bool ExtendedSpaceForDecisions = false;

    public override string GetDialogue() {
        return dialogue;
    }

    public override TMP_FontAsset GetFontAsset() {
        return fontAsset;
    }

    public string[] GetDecisions() {
        int i = 0;
        foreach (string s in Decisions) if (!string.IsNullOrEmpty(s)) i++; //count empty decisions
        string[] nonEmptyDecisions = new string[i]; //new string array
        for (i = 0; i < 4; i++) if (!string.IsNullOrEmpty(Decisions[i])) nonEmptyDecisions[i] = Decisions[i]; //put them in. In case not as upward in the list as can be it's ok
        return nonEmptyDecisions;
    }
}
