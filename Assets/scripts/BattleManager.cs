using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour{
    [SerializeField] private GameObject _character;

    private void Start() {
        GameEvents.Instance.EndBattle += EndBattle;

        switch (GameManager.Instance.NextEnemyBattle) {
            case EnemyBattle.Max:
                gameObject.AddComponent<MaxBattle>();
                break;
            case EnemyBattle.TestEnemy:
                gameObject.AddComponent<TestEnemyBattle>();
                break;
        }
    }

    private void EndBattle() {
        GameManager.Instance.ChangeGameState(GameState.overworld);

    }
}

[Serializable]
public enum EnemyBattle {
    Max = 0,
    TestEnemy = 1
}