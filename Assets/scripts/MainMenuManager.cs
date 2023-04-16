using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

//Handles the main menu and the intro cutscene.
public class MainMenuManager : Singleton<MainMenuManager>{

    //****************************//
    //                            //
    //         VARIABLES          //
    //                            //
    //****************************//

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

    //****************************//
    //                            //
    //      MENU NAVIGATION       //
    //                            //
    //****************************//

    //Subscribbing to events and calculating values
    private new void Awake() {
        base.Awake();
        maxOption = options.Length - 1;

        GameEvents.Instance.StartIntro += StartIntro;
        GameEvents.Instance.OnGameStateChange += GoToMainMenu;

        _canvas = GetComponent<Canvas>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnDestroy() {
        if(GameEvents.Instance != null) {
            GameEvents.Instance.StartIntro -= StartIntro;
            GameEvents.Instance.OnGameStateChange -= GoToMainMenu;
        }
    }

    // Called by unity input system events when the menu is navigated
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

    // Handles when an option is selected
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

    //****************************//
    //                            //
    //   INTRO CUTSCENE-RELATED   //
    //                            //
    //****************************//

    // Handles playing the intro cutscene when a new game is started
    public void StartIntro() {
        _canvas.enabled = false;
        _playerInput.SwitchCurrentActionMap("Cutscene");
        GameManager.Instance.ChangeGameState(GameState.cutscene);
        _introCutsceneCanvas.SetActive(true);
        _introCutsceneTimeline.SetActive(true);
    }

    // Cleans up intro cutscene
    public void EndCutscene() {
        _introCutsceneCanvas.SetActive(false);
        _introCutsceneTimeline.SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.mainMenu);
    }
    private void GoToMainMenu(GameState newGameState) {
        if (newGameState == GameState.mainMenu) {
            _canvas.enabled = true;
            _playerInput.SwitchCurrentActionMap("UI");
        }
    }
}

