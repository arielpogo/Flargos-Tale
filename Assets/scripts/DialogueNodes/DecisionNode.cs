using UnityEngine;
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

    public string[] GetDecisions() {
        string[] decisions = new string[4];
        decisions[0] = decision1;
        decisions[1] = decision2;
        decisions[2] = decision3;
        decisions[3] = decision4;
        return decisions;
    }

    //TODO: Dynamic ports
    public string decision1, decision2, decision3, decision4;
}