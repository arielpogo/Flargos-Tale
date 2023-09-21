using UnityEngine;

/// <summary>
/// Moves the player to the appropriate spot when entering a level.
/// </summary>
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

    //called by GameManager
    private void SpawnPlayer(GameObject Player, int id, Vector3 offset){
        if (id == _spawnerID) {
            Player.transform.position = transform.position + offset;
        }
    }
}
