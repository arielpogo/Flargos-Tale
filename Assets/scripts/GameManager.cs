using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The game manager handles the gamestate and the startup.
/// </summary>
public class GameManager : PersistentSingleton<GameManager> {
    public GameState MasterGameState { get; private set; }
    public GameObject Player { get; private set; }

    public int NextEnemyBattle { get; private set; }

    private new void Awake() {
        base.Awake();
        Screen.SetResolution(640, 480, false);
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    private void OnDestroy() {
        Debug.Log($"I am getting destroyed, my player is {Player}");
        if (GameEvents.Instance != null) {
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

    public void LoadGame() {
        SceneManager.LoadScene(SaveManager.PlayerData.sceneName);
    }

    private void OnSceneChange(Scene Current, Scene Next) {
        Player = GameObject.FindWithTag("Player");
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