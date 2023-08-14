using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The game manager handles the gamestate and the startup.
/// </summary>
public class GameManager : PersistentSingleton<GameManager> {
    private Stack<GameState> _gameStateStack = new();
    public GameObject Player { get; private set; }

    public int NextEnemyBattle { get; private set; }

    private new void Awake() {
        base.Awake();
        Application.targetFrameRate = 60;
        Screen.SetResolution(640, 480, false);
        SceneManager.activeSceneChanged += OnSceneChange;
        GameEvents.Instance.MajorEvent += MajorEventHandler;
    }

    private void OnDestroy() {
        Debug.Log($"I am getting destroyed, my player is {Player}");
        if (GameEvents.Instance != null) {
            GameEvents.Instance.MajorEvent -= MajorEventHandler;
        }
    }

    public void AddGameState(GameState newState) {
        Debug.Log($"Adding gamestate {newState}");
        _gameStateStack.Push(newState);
        GameEvents.Instance.OnGameStateChange?.Invoke();
    }

    private void RevertGameState(){
        Debug.Log($"Popping gamestate {_gameStateStack.Peek()}");
        _gameStateStack.Pop();
        //if (_gameStateStack.Count == 0) _gameStateStack.Push(GameState.overworld_control);
        GameEvents.Instance.OnGameStateChange?.Invoke();
    }

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
                    Debug.LogError($"Attempt to revert from {GetCurrentGameState()} due to major event {e}");
                    return;
                }
                else RevertGameState();
                break;
            case MajorEvent.ui_closed:
                if (GetCurrentGameState() != GameState.ui)
                {
                    Debug.LogError($"Attempt to revert from {GetCurrentGameState()} due to major event {e}");
                    return;
                }
                else RevertGameState();
                break;
            case MajorEvent.dialogue_ended:
                if (GetCurrentGameState() != GameState.dialogue)
                {
                    Debug.LogError($"Attempt to revert from {GetCurrentGameState()} due to major event {e}");
                    return;
                }
                else RevertGameState();
                break;
            case MajorEvent.battle_ended:
                if (GetCurrentGameState() != GameState.battle)
                {
                    Debug.LogError($"Attempt to revert from {GetCurrentGameState()} due to major event {e}");
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

    public void LoadGame() {
        SceneManager.LoadScene(SaveManager.PlayerData.sceneName);
    }

    private void OnSceneChange(Scene Current, Scene Next) {
        Player = GameObject.FindWithTag("Player");
        //place player where they need to be
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