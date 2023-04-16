using System;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// The game manager handles the gamestate and the startup.
/// </summary>
public class GameManager : PersistentSingleton<GameManager> {
    public GameState MasterGameState { get; private set; }
    public GameObject player; //set in the editor

    private void Start() {
        if(SceneManager.GetActiveScene().name == "mainMenu") HandleStartup();
    }

    /// <summary>
    /// Handles the changes in gameplay when the game state changes
    /// </summary>
    /// <param name="newState">The state to change the game to</param>
    /// <exception cref="ArgumentOutOfRangeException">When a GameState without logic is passed</exception>
    public void ChangeGameState(GameState newState) {
        if (newState == MasterGameState) return; //no change needed
        MasterGameState = newState;
        GameEvents.Instance.OnGameStateChange?.Invoke(newState);
    }

    /// <summary>
    /// Determines whether to immediately show intro cutscene or show the save screen
    /// </summary>
    private void HandleStartup() {
        Screen.SetResolution(640, 480, false);
        if (SaveManager.Instance.SaveFiles.Length == 0) {
            GameEvents.Instance.StartIntro?.Invoke(); //trigger the StartIntro event
        }
        else {
            ChangeGameState(GameState.mainMenu);
        }
    }
}

[Serializable]
public enum GameState {
    startup = 0,
    mainMenu = 1,
    overworld = 2,
    overworldMenu = 3,
    dialogue = 4,
    cutscene = 5,
    cutscene_with_control = 6
}