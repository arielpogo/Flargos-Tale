using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour{
    public int goToSceneIndex = 0;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")) {
            SceneManager.LoadScene(goToSceneIndex, LoadSceneMode.Single);
       }
    }
}