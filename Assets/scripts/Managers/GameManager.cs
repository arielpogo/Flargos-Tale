using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The game manager handles the gamestate and the startup.
/// </summary>
public class GameManager : PersistentSingleton<GameManager> {
    //Gamestate
    private Stack<GameState> _gameStateStack = new();

    //Global reference to the player
    public GameObject Player { get; private set; }
    
    //Changing Levels
    public int NextSpawner //if set, then use it. Else, spawn the player in the default position
    {
        get => _nextSpawner;
        set {
            _useSpawner = true;
            _nextSpawner = value;
        }
    }
    public Vector3 SpawnerOffset;
    private bool _useSpawner = false;
    private int _nextSpawner = 0;

    //****************************//
    //                            //
    //      AWAKE/ONDESTROY       //
    //                            //
    //****************************//

    private new void Awake() {
        base.Awake();
        Application.targetFrameRate = 60;
        Screen.SetResolution(640, 480, false);
        SceneManager.sceneLoaded += OnSceneChange;
        GameEvents.Instance.MajorEvent += MajorEventHandler;
    }

    private void OnDestroy() {
        Debug.Log($"I am getting destroyed, my player is {Player}");
        if (GameEvents.Instance != null) {
            GameEvents.Instance.MajorEvent -= MajorEventHandler;
            SceneManager.sceneLoaded -= OnSceneChange;
        }
    }

    //****************************//
    //                            //
    //         GAMESTATE          //
    //                            //
    //****************************//

    public void AddGameState(GameState newState) {
        Debug.Log($"Adding gamestate {newState}");
        _gameStateStack.Push(newState);
        GameEvents.Instance.OnGameStateChange?.Invoke();
    }

    private void RevertGameState(){
        Debug.Log($"Popping gamestate {_gameStateStack.Peek()}");
        _gameStateStack.Pop();
        GameEvents.Instance.OnGameStateChange?.Invoke();
    }

    private void LogMajorEventMismatchError(MajorEvent e) {
        Debug.LogError($"Attempt to revert from {GetCurrentGameState()} due to major event {e}");
    }

    //there is probably a better way to do this...
    private void MajorEventHandler(MajorEvent e) {
        Debug.Log($"MajorEventHandler called with event {e}");
        switch (e) {
            case MajorEvent.cutscene_started:
                AddGameState(GameState.cutscene);
                break;
            case MajorEvent.ui_opened:
                AddGameState(GameState.ui);
                break;
            case MajorEvent.dialogue_started:
                AddGameState(GameState.dialogue);
                break;
            case MajorEvent.battle_started:
                AddGameState(GameState.battle);
                break;
            case MajorEvent.cutscene_ended:
                if (GetCurrentGameState() != GameState.cutscene) {
                    LogMajorEventMismatchError(e);
                    return;
                }
                else RevertGameState();
                break;
            case MajorEvent.ui_closed:
                if (GetCurrentGameState() != GameState.ui){
                    LogMajorEventMismatchError(e);
                    return;
                }
                else RevertGameState();
                break;
            case MajorEvent.dialogue_ended:
                if (GetCurrentGameState() != GameState.dialogue){
                    LogMajorEventMismatchError(e);
                    return;
                }
                else RevertGameState();
                break;
            case MajorEvent.battle_ended:
                if (GetCurrentGameState() != GameState.battle){
                    LogMajorEventMismatchError(e);
                    return;
                }
                else RevertGameState();
                break;
            case MajorEvent.overworld_enter: //This may cause issues in the future
                Debug.Log($"Clearing gamestate stack and placing overworld");
                _gameStateStack.Clear();
                _gameStateStack.Push(GameState.overworld_control);
                break;
        }
    }

    public GameState GetCurrentGameState(){
        return _gameStateStack.Peek();
    }

    //****************************//
    //                            //
    //            MISC            //
    //                            //
    //****************************//

    public void LoadGame() {
        SceneManager.LoadScene(SaveManager.PlayerData.sceneName);
    }

    private void OnSceneChange(Scene a, LoadSceneMode next) {
        Player = GameObject.FindWithTag("Player");
        if (Player == null) return;
        else if (_useSpawner){
            GameEvents.Instance.SpawnPlayer.Invoke(Player, _nextSpawner, SpawnerOffset);
            _useSpawner = false;
        }
        else if (_nextSpawner == -999) { //flag 
            Player.transform.position = SaveManager.PlayerData.savedPos;
        }
    }
}

[Serializable]
public enum GameState {
    overworld_control,
    ui,
    dialogue,
    cutscene,
    battle
}

public enum MajorEvent {
    ui_opened,
    ui_closed,
    cutscene_started,
    cutscene_ended,
    dialogue_started,
    dialogue_ended,
    battle_started,
    battle_ended,
    overworld_enter
}