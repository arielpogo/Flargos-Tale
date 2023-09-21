using UnityEngine;

/// <summary>
/// NPC that starts a battle when spoken to.
/// </summary>
public class NPCBattleStarter : BaseInteractableClass {
    [SerializeField] int battleID;

    public override void Interact() {
        BattleManager.Instance.StartBattle(battleID);
    }
}
