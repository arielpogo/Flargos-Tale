using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : Singleton<MainMenuManager>{
    public TextMeshProUGUI[] options = new TextMeshProUGUI[3];
    private Color idle = Color.white;
    private Color highlight = new(1, 1, 0, 1);

    private Vector2 direction;

    private int selected = -1; // -1 == unselected so far
    private int maxOption;

    [SerializeField] private GameObject _introCutsceneCanvas;
    [SerializeField] private GameObject _introCutsceneTimeline;

    private Canvas _canvas;
    private PlayerInput _playerInput;

    /// <summary>
    /// Setup, specifically subscribbing to events and calculating values
    /// </summary>
    private new void Awake() {
        base.Awake();
        maxOption = options.Length - 1;

        _canvas = GetComponent<Canvas>();
        _playerInput = GetComponent<PlayerInput>();
    }

    /// <summary>
    /// Called by unity input system events when the menu is navigated
    /// </summary>
    /// <param name="context">Standard unity input system event param</param>
    public void OnNavigate(InputAction.CallbackContext context) {
        if(context.started) {
            direction = context.ReadValue<Vector2>();
            if (selected == -1) {
                if(direction.y > 0) {selected = maxOption;}
                else if (direction.y < 0) { selected = 0; }
            }
            else {
                if (direction.y < 0) {selected++;}
                else if (direction.y > 0) {selected--;}

                if (selected < 0) {selected = maxOption;}
                else if (selected > maxOption) {selected = 0;}
            }

            for (int i = 0; i <= maxOption; i++) {
                if (i == selected) { options[i].color = highlight; }
                else { options[i].color = idle; }
            }
        }
    }

    /// <summary>
    /// Handles when an option is selected
    /// </summary>
    /// <param name="context">Default unity input system param</param>
    public void OnSelect(InputAction.CallbackContext context) {
        if (context.started) {
            switch (selected) {
                case 0:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                case 1:
                    break;
                case 2:
                    Application.Quit();
                    break;
            }
        }
    }

    /// <summary>
    /// Handles playing the intro cutscene when a new game is started
    /// </summary>
    public void StartIntro() {
        _canvas.enabled = false;
        _playerInput.SwitchCurrentActionMap("Cutscene");
        GameManager.Instance.ChangeGameState(GameState.cutscene);
        _introCutsceneCanvas.SetActive(true);
        _introCutsceneTimeline.SetActive(true);
    }
    /// <summary>
    /// Cleans up intro cutscene
    /// </summary>
    public void EndCutscene() {
        _introCutsceneCanvas.SetActive(false);
        _introCutsceneTimeline.SetActive(false);
        GoToMainMenu();
    }
    private void GoToMainMenu() {
       _canvas.enabled = true;
       _playerInput.SwitchCurrentActionMap("UI");
    }
}

