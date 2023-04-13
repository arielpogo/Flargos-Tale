using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class OverworldMenuManager : MonoBehaviour {
    public TextMeshProUGUI[] options = new TextMeshProUGUI[3];
    private Color idle = Color.white;
    private Color highlight = new Color(1, 1, 0, 1);

    private Vector2 direction;

    private int selected = -1; // -1 == unselected so far

    private int maxOption;

    private void Start() {
        maxOption = options.Length - 1;
    }

    public void OnNavigate(InputAction.CallbackContext context) {
        if (context.performed) {
            direction = context.ReadValue<Vector2>();
            if (selected == -1) {
                if (direction.y > 0) { selected = maxOption; }
                else if (direction.y < 0) { selected = 0; }
            }
            else {
                if (direction.y < 0) { selected++; }
                else if (direction.y > 0) { selected--; }

                if (selected < 0) { selected = maxOption; }
                else if (selected > maxOption) { selected = 0; }
            }

            for (int i = 0; i <= maxOption; i++) {
                if (i == selected) { options[i].color = highlight; }
                else { options[i].color = idle; }
            }
        }
    }

    public void OnCloseMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            for (int i = 0; i <= maxOption; i++) options[i].color = idle;
            selected = -1;
            GameManager.Instance.ChangeGameState(GameState.overworld);
        }
    }

    public void OnSelect(InputAction.CallbackContext context) {
        if (context.performed) {
            switch (selected) {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }
    }
}

