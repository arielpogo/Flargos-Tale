using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Brings player between scenes
/// </summary>
public class LevelChanger : MonoBehaviour{
    [SerializeField] private int _toScene = 0;
    [SerializeField] private int _toSpawner = 0;
    [SerializeField] private LevelChangerDirection _direction;
   
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")) {
            Vector3 relativePosition = transform.InverseTransformPoint(collision.transform.position);

            switch (_direction) {
                case LevelChangerDirection.Up:
                case LevelChangerDirection.Down:
                    GameManager.Instance.SpawnerOffset = new(relativePosition.x, 0, 0);
                    break;
                case LevelChangerDirection.Left:
                case LevelChangerDirection.Right:
                    GameManager.Instance.SpawnerOffset = new(0, relativePosition.y, 0);
                    break;
            }

            GameManager.Instance.NextSpawner = _toSpawner;
            SceneManager.LoadScene(_toScene);
            //gamemanger picks up that the scene was changed and calls a delegate subscribed by all spawners with NextSpawner id and the found player
       }
    }
}

public enum LevelChangerDirection {
    Up,
    Down,
    Left,
    Right
}