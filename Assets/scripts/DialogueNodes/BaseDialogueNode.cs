using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using XNode;

public abstract class BaseDialogueNode : Node {
    /// <summary>
    /// If a node on a DialogueGraph has dialogue to return, it will override this method and do that. Otherwise it will return null.
    /// </summary>
    /// <returns>Dialogue, if it has any (StartNodes don't, for example), or null.</returns>
    public virtual string GetDialogue() {
        return null;
    }

    /// <summary>
    /// If a node on a DialogueGraph has a portrait to return, it will override this method and do that. Otherwise it will return null.
    /// </summary>
    /// <returns>A portrait, if it has any (EndNodes don't, for example), or null.</returns>
    public virtual Sprite GetPortrait() {
        return null;
    }

    /// <summary>
    /// If a node on a DialogueGraph has a fontAsset to return, it will override this method and do that. Otherwise it will return null.
    /// </summary>
    /// <returns>A TMP_FontAsset, if it has any (EndNodes don't, for example), or null.</returns>
    public virtual TMP_FontAsset GetFontAsset() {
        return null;
    }

    //I don't plan on using this, but xNode requires it or else it throws a fit
    public override object GetValue(NodePort port) { return null; }
}