using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxBattle : Battle {
    private void Start() {
        BattleTheme = Resources.Load<AudioClip>("maxtheme");
    }
}
