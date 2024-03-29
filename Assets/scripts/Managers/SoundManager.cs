using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager for audio
/// </summary>
public class SoundManager : PersistentSingleton<SoundManager> {

    private AudioSource _musicSource; //component for music
    private AudioSource _SfxSource; //component for sfx
    public const float defaultFadeTime = 0.5f;

    //****************************//
    //                            //
    //      AWAKE/ONDESTROY       //
    //                            //
    //****************************//


    private new void Awake() {
        base.Awake();
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;

        _SfxSource = gameObject.AddComponent<AudioSource>();

        GameEvents.Instance.PlaySong += PlaySongStarter;
        GameEvents.Instance.PlaySfx += PlaySfx;
        GameEvents.Instance.StopMusic += StopMusicStarter;
        SceneManager.sceneLoaded += GetSceneMusic;
    }

    private void OnDestroy() {
        if(GameEvents.Instance != null) {
            GameEvents.Instance.PlaySong -= PlaySongStarter;
            GameEvents.Instance.PlaySfx -= PlaySfx;
            GameEvents.Instance.StopMusic -= StopMusicStarter;
            SceneManager.sceneLoaded -= GetSceneMusic;
        }
    }

    //****************************//
    //                            //
    //          HELPERS           //
    //                            //
    //****************************//

    private void PlaySongStarter(AudioClip sound, float volume, float fadeInTime, float fadeOutTime) => StartCoroutine(PlaySong(sound,volume,fadeInTime,fadeOutTime));
    private void StopMusicStarter(float fadeOutTime) => StartCoroutine(StopMusic(fadeOutTime));

    //****************************//
    //                            //
    //         COROUTINES         //
    //                            //
    //****************************//

    //changes/starts playing a song
    private IEnumerator PlaySong(AudioClip song, float volume, float fadeInTime, float fadeOutTime) {
        yield return StartCoroutine(StopMusic(fadeOutTime));
        _musicSource.clip = song;
        _musicSource.Play();
        StartCoroutine(LerpVolume(0, volume, fadeInTime));
    }

    private IEnumerator StopMusic(float fadeOutTime) {
        if (!_musicSource.isPlaying) yield break;
        yield return StartCoroutine(LerpVolume(_musicSource.volume, 0, fadeOutTime));
        _musicSource.Stop();
    }

    /// <summary>
    /// Interpolates volume
    /// </summary>
    private IEnumerator LerpVolume(float start, float end, float duration) {
        float t = 0f;
        while(t < duration) {
            _musicSource.volume = Mathf.Lerp(start, end, t/duration);
            t += Time.deltaTime;
            yield return null;
        }
        _musicSource.volume = end;
    }

    private void PlaySfx(AudioClip sfx, float volume) {
        _SfxSource.volume = volume;
        _SfxSource.PlayOneShot(sfx);
    }

    //****************************//
    //                            //
    //            MISC            //
    //                            //
    //****************************//

    /// <summary>
    /// Triggered by SceneManager.sceneLoaded, changes music based on scene
    /// </summary>
    private void GetSceneMusic(Scene current, LoadSceneMode m) {
        string song = string.Empty;
        switch (current.buildIndex) {
            case 1:
            case 2:
            case 3:
                song = "bpc_music_temp";
                break;
            case 4:
                song = "grass";
                break;
            case 5:
                song = "kisses_village";
                break;
        }

        if (song.Length > 0 && _musicSource.isPlaying && _musicSource.clip.name != song) StartCoroutine(PlaySong(Resources.Load<AudioClip>("music/" + song), 1f, 0f, 0.25f));
        else if (song.Length > 0 && !_musicSource.isPlaying) StartCoroutine(PlaySong(Resources.Load<AudioClip>("music/" + song), 1f, 0f, 0.25f));
    }
}
