using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Runtime.CompilerServices;

/// <summary>
/// The game manager handles the gamestate and the startup.
/// </summary>
public class GameManager : PersistentSingleton<GameManager> {
    public GameState MasterGameState { get; private set; }
    public GameObject player; //set in the editor

    public EnemyBattle NextEnemyBattle { get; private set; }

    private void Start() {
        Screen.SetResolution(640, 480, false);
        GameEvents.Instance.StartBattle += SetBattle;
    }

    private void OnDestroy() {
        if (GameEvents.Instance != null) {
            GameEvents.Instance.StartBattle -= SetBattle;
        }
    }

    /// <summary>
    /// Handles the changes in gameplay when the game state changes
    /// </summary>
    /// <param name="newState">The state to change the game to</param>
    public void ChangeGameState(GameState newState) {
        if (newState == MasterGameState) return; //no change needed
        MasterGameState = newState;
        GameEvents.Instance.OnGameStateChange?.Invoke(newState);
    }

    private void SetBattle(EnemyBattle Enemy) {
        NextEnemyBattle = Enemy;
        ChangeGameState(GameState.battle);
    }

    public void LoadGame() {
        SceneManager.LoadScene(SaveManager.PlayerData.sceneName);
    }

}

[Serializable]
public enum GameState {
    startup = 0,
    overworld = 1,
    overworldMenu = 2,
    dialogue = 3,
    cutscene = 4,
    cutscene_with_control = 5,
    battle = 6
}