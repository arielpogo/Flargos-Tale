using UnityEngine;

public class NPCBattleStarter : BaseInteractableClass {
    [SerializeField] private EnemyBattle Battle;

    public override void Interact() {
        GameEvents.Instance.StartBattle(Battle);
    }
}
