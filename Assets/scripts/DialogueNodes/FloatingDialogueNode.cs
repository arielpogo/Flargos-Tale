using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using XNode;

/// <summary>
/// A screen of dialogue which has no choices for the player to make, not in a container.
/// </summary>
public class FloatingDialogueNode : BaseDialogueNode {
    [Input] public int Previous;
    [Output] public int Next;
    public string dialogue;
    public Sprite portrait;
    public TMP_FontAsset fontAsset;

    public override string GetDialogue() {
        return dialogue;
    }

    public override Sprite GetPortrait() {
        return portrait;
    }
    public override TMP_FontAsset GetFontAsset() {
        return fontAsset;
    }
}