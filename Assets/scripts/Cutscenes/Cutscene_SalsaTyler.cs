using UnityEngine;
using UnityEngine.Playables;

public class Cutscene_SalsaTyler : MonoBehaviour {
    private AudioClip song;
    private int cutsceneProgress = 0;
    //private bool paused = false;

    private void Awake() {
        song = Resources.Load("music/salsa") as AudioClip;
        song.LoadAudioData();
    }

    void OnTriggerEnter2D(Collider2D other) { //cutscene start
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<PlayableDirector>().enabled = true;
        GameEvents.Instance.StopMusic.Invoke();
        GameEvents.Instance.MajorEvent.Invoke(MajorEvent.cutscene_started);
    }

    public void Proceed() {
        cutsceneProgress++;
        switch (cutsceneProgress) {
            case 1:
                GameEvents.Instance.PlaySong.Invoke(song);
                break;
        }
    }

    /*public void TogglePause() {
        if (paused) {
            paused = false;
            GameManager.Instance.ChangeGameState(GameState.dialogue);
        }
        else {
            paused = true;
            GameManager.Instance.ChangeGameState(GameState.cutscene);
        }
    }*/

    public void EndCutscene() {
        GameEvents.Instance.MajorEvent.Invoke(MajorEvent.cutscene_ended);
        BattleManager.Instance.StartBattle(0);
    }
}
