using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which holds game events.
/// </summary>
public class GameEvents : PersistentSingleton<GameEvents> {
    public delegate void GameStateChange(GameState NewGameState); //for when the game state changes
    public GameStateChange OnGameStateChange;
}
