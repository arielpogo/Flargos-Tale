using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Class which holds game events.
/// </summary>
public class GameEvents : PersistentSingleton<GameEvents> {
    //Gamestate
    public Action OnGameStateChange; //to notify other scripts the gamestate changed

    //Major Events
    public delegate void MajorEventDelegate(MajorEvent evnt);
    public MajorEventDelegate MajorEvent;

    //Sound
    public delegate void PlaySongDelegate(AudioClip sound, float volume = 1f, float fadeInTime = 0f, float fadeOutTime = 0f); //for playing songs
    public PlaySongDelegate PlaySong;
    public delegate void OneShotDelegate(AudioClip sound, float volume = 1f); //for one shot sfx
    public OneShotDelegate PlaySfx;
    public delegate void SoundEndDelegate(float fadeOutTime = SoundManager.defaultFadeTime); //for ending sounds
    public SoundEndDelegate StopMusic;

    //Battles
    public Action BattleStart;
    public Action EndBattle;

    //Saving
    public Action OnDecideSave;
    public Action OnSaved;
    public delegate void LoadSaveDelegate(int save);
    public LoadSaveDelegate LoadSave;
}
