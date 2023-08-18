using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour {
    [SerializeField] private int _spawnerID;
    private void Awake(){
        GameEvents.Instance.SpawnPlayer += SpawnPlayer;
    }

    private void OnDestroy() {
        if (GameEvents.Instance != null) {
            GameEvents.Instance.SpawnPlayer -= SpawnPlayer;
        }
    }

    private void SpawnPlayer(GameObject Player, int id, Vector3 offset){
        if (id == _spawnerID) {
            Player.transform.position = transform.position + offset;
        }
    }
}
