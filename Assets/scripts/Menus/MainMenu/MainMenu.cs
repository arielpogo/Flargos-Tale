using System.Text.RegularExpressions;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;

//Handles the main menu and the intro cutscene.
public class MainMenuManager : NavigableMenu{

    //****************************//
    //                            //
    //         VARIABLES          //
    //                            //
    //****************************//

    [SerializeField] private GameObject _introCutsceneCanvas;
    [SerializeField] private GameObject _introCutsceneTimeline;

    [SerializeField] private AudioClip _mainMenuSong;

    private Canvas _canvas;
    private PlayerInput _playerInput;

    //Subscribbing to events and calculating values
    private void Awake() {
        _canvas = GetComponent<Canvas>();
        _playerInput = GetComponent<PlayerInput>();

        if (SaveManager.Instance.SaveFiles.Length == 0) StartIntro();
        else GoToMainMenu();
    }

    private void RefreshSaves() { 
        for(int i = 0; i < SaveManager.maxSaveFiles; i++) {
            SaveManager.Instance.Load(i);
            if (SaveManager.PlayerData.status == 0) { //valid
                //set the date child of each TMPText on the main menu
                _columns[0].Rows[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SaveManager.PlayerData.date.ToShortDateString();
                //set the time child of each TMPText on the main menu
                _columns[0].Rows[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = SaveManager.PlayerData.date.ToShortTimeString();
                //Save1, Save2 ...
                _columns[0].Rows[i].text = "Save" + (i + 1); //0 indexed
            }
            else if(SaveManager.PlayerData.status == 1) { //DNE
                _columns[0].Rows[i].text = "Empty";
                _columns[0].Rows[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = String.Empty;
                _columns[0].Rows[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Empty;
            }
            else if(SaveManager.PlayerData.status == 2) { //Corrupted or some other random file renamed and placed in saves folder
                _columns[0].Rows[i].text = "Error";
                _columns[0].Rows[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "UNABLE";
                _columns[0].Rows[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "TO READ";
            }
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
        SaveManager.Instance.Load(_currentRow);
        if(SaveManager.PlayerData.status == 0) {
            GameManager.Instance.LoadGame();
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
        //_introCutsceneCanvas.SetActive(false);
        //_introCutsceneTimeline.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private void GoToMainMenu() {
        _canvas.enabled = true;
        _playerInput.SwitchCurrentActionMap("UI");
        GameEvents.Instance.PlaySong?.Invoke(_mainMenuSong);
        RefreshSaves();
    }
}

