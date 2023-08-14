using System.Collections;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager> {

    private AudioSource _musicSource;
    private AudioSource _SfxSource;
    public const float defaultFadeTime = 0.5f;

    private new void Awake() {
        base.Awake();
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;

        _SfxSource = gameObject.AddComponent<AudioSource>();

        GameEvents.Instance.PlaySong += PlaySongStarter;
        GameEvents.Instance.PlaySfx += PlaySfx;
        GameEvents.Instance.StopMusic += StopMusicStarter;
    }

    private void OnDestroy() {
        if(GameEvents.Instance != null) {
            GameEvents.Instance.PlaySong -= PlaySongStarter;
            GameEvents.Instance.PlaySfx -= PlaySfx;
            GameEvents.Instance.StopMusic -= StopMusicStarter;
        }
    }

    private void PlaySongStarter(AudioClip sound, float volume, float fadeInTime, float fadeOutTime) => StartCoroutine(PlaySong(sound,volume,fadeInTime,fadeOutTime));
    private void StopMusicStarter(float fadeOutTime) => StartCoroutine(StopMusic(fadeOutTime));

    //changes/starts playing a song
    private IEnumerator PlaySong(AudioClip song, float volume, float fadeInTime, float fadeOutTime) {
        yield return StartCoroutine(StopMusic(fadeOutTime));
        _musicSource.clip = song;
        _musicSource.Play();
        StartCoroutine(LerpVolume(0, volume, fadeInTime));
    }

    private IEnumerator StopMusic(float fadeOutTime) {
        yield return StartCoroutine(LerpVolume(_musicSource.volume, 0, fadeOutTime));
        _musicSource.Stop();
    }

    private IEnumerator LerpVolume(float start, float end, float duration) {
        float t = 0f;
        while(t < duration) {
            _musicSource.volume = Mathf.Lerp(start, end, t/duration);
            t += Time.deltaTime;
            yield return null;
        }
        _musicSource.volume = end;
    }

    public void PlaySfx(AudioClip sfx, float volume) {
        _SfxSource.volume = volume;
        _SfxSource.PlayOneShot(sfx);
    }

}
