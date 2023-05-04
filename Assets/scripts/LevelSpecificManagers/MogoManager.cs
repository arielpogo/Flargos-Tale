using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MogoManager : MonoBehaviour{
    [SerializeField] private AudioClip mogoSong;

    private void Start() {
        GameEvents.Instance.PlaySong?.Invoke(mogoSong);
    }
}
