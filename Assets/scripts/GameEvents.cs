using System;
using UnityEngine;

/// <summary>
/// Class which holds game events.
/// </summary>
public class GameEvents : PersistentSingleton<GameEvents> {
    //Gamestate
    public delegate void GameStateChange(GameState NewGameState); //for when the game state changes
    public GameStateChange OnGameStateChange;

    //Sound
    public delegate void SoundEvent(AudioClip sound); //for playing sounds
    public SoundEvent PlaySong;
    public SoundEvent PlaySfx;

    //Battles
    public delegate void BattleStart(EnemyBattle Enemy);
    public BattleStart StartBattle;
    public Action EndBattle;

    //Saving
    public Action OnDecideSave;
    public Action OnSaved;
}
