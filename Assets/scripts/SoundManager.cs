using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager> {

    private AudioSource _musicSource;
    private AudioSource _SfxSource;

    private new void Awake() {
        base.Awake();
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;

        _SfxSource = gameObject.AddComponent<AudioSource>();

        GameEvents.Instance.PlaySong += PlaySong;
        GameEvents.Instance.PlaySfx += PlaySfx;
    }

    private void OnDestroy() {
        if(GameEvents.Instance != null) {
            GameEvents.Instance.PlaySong -= PlaySong;
            GameEvents.Instance.PlaySfx -= PlaySfx;
        }
    }

    //changes/starts playing a song
    public void PlaySong(AudioClip song) {
        _musicSource.Stop();
        _musicSource.clip = song;
        _musicSource.Play();
    }

    public void StopMusic() {
        _musicSource.Stop();
    }

    public void PlaySfx(AudioClip sfx) => _SfxSource.PlayOneShot(sfx);

}
