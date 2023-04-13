using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The game manager handles what happens when the state of the game changes
/// </summary>
public class GameManager : PersistentSingleton<GameManager> {
    public GameState MasterGameState { get; private set; }
    public GameObject player;

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

        switch (newState) {
            case GameState.mainMenu:
                switch (MasterGameState) {
                    case GameState.startup:
                        FromStartupToMainMenu();
                        break;
                    case GameState.overworld:
                        break;
                }
                break;

            case GameState.overworld:
                if (player == null) Debug.Log("No player selected for this scene (this is ok if there is supposed to be no player to control here).");
                //detect what state we're switching from
                switch (MasterGameState) {
                    case GameState.mainMenu:
                        FromMainMenuToOverworld();
                        break;
                    case GameState.overworldMenu:
                        FromOverworldMenuToOverworld();
                        break;
                    case GameState.dialogue:
                        FromDialogueToOverworld();
                        break;
                }
                break;

            case GameState.overworldMenu:
                FromOverworldToOverworldMenu();
                break;

            case GameState.cutscene:
                switch (MasterGameState) {
                    case GameState.startup:
                        break;
                    case GameState.overworld:
                        FromOverworldToCutscene();
                        break;
                }
                break;
            case GameState.dialogue:
                FromOverworldToDialogue();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        MasterGameState = newState;
    }

    /// <summary>
    /// Determines whether to immediately show intro cutscene or show the save screen
    /// </summary>
    private void HandleStartup() {
        Screen.SetResolution(640, 480, false);
        if (SaveManager.Instance.SaveFiles.Length == 0) {
            MainMenuManager.Instance.StartIntro(); //trigger the StartIntro event
        }
        else {
            ChangeGameState(GameState.mainMenu);
        }
    }
    
    /// <summary>
    /// When startup and setup is finished (thus at least one save exists)
    /// </summary>
    private void FromStartupToMainMenu() {

    }

    /// <summary>
    /// Handles loading a save file
    /// </summary>
    private void FromMainMenuToOverworld() {
        
    }

    /// <summary>
    /// Handles pausing control when the ingame menu is opened
    /// </summary>
    private void FromOverworldToOverworldMenu() {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI"); //Change this so that the player controller does this, later
    }

    /// <summary>
    /// Handles closing the ingame menu
    /// </summary>
    private void FromOverworldMenuToOverworld() {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Overworld"); //Ditto
    }

    private void FromOverworldToCutscene() {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Cutscene"); // Ditto
    }


    private void FromOverworldToDialogue() {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Dialogue"); //Ditto, I guess?
    }
    private void FromDialogueToOverworld() {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Overworld");
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
}