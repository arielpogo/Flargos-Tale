using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class DialogueGraph : NodeGraph {
    /// <summary>
    /// Finds the StartNode of the DialogueGraph, which is the entry point.
    /// </summary>
    /// <returns>The StartNode of the DialogueGraph</returns>
    public StartNode GetStartNode() {
        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i] is StartNode) return nodes[i] as StartNode;
        }
        return null; //if none is found
    }
}