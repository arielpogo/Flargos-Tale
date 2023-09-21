using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTestingSaving : BaseInteractableClass
{
    public override void Interact() {
        Debug.Log(SaveManager.Instance.RemoveMoney(0, 10));
        SaveManager.PlayerData.strength += 10;
        SaveManager.PlayerData.stealth -= 10;
    }
}
