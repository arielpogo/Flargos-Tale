using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Handles the main menu and the intro cutscene.
public class MainMenuManager : NavigableMenu{

    //****************************//
    //                            //
    //         VARIABLES          //
    //                            //
    //****************************//

    [SerializeField] private GameObject _introCutsceneCanvas;
    [SerializeField] private GameObject _introCutsceneTimeline;

    private Canvas _canvas;
    private PlayerInput _playerInput;

    //Subscribbing to events and calculating values
    private void Awake() {
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
    //****************************//
    //                            //
    //      MENU NAVIGATION       //
    //                            //
    //****************************//

    public override void OnCloseMenu() {
        
    }

    public override void OnReturn(InputValue value) { 
    }


    // Handles when an option is selected
    public override void OnSubmit() {
        switch (_currentRow) {
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

