using UnityEngine;

public class MaxBattle : Battle {
    private void Start() {
        BattleTheme = Resources.Load<AudioClip>("maxtheme");
    }
}
