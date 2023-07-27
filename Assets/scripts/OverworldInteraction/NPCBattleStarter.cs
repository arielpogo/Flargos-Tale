using UnityEngine;

public class NPCBattleStarter : BaseInteractableClass {
    [SerializeField] int battleID;

    public override void Interact() {
        BattleManager.Instance.StartBattle(battleID);
    }
}
