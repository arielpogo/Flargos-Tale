using System;
using System.Collections;
using TMPro;
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

    //Update the saves when they change, 
    private void RefreshSaves() {
        int emptySaveCounter = 0; //immediately start the intro if all are empty, ex. all saves are erased
        for(int i = 0; i < SaveManager.maxSaveFiles; i++) {
            GameEvents.Instance.LoadSave.Invoke(i);
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
                emptySaveCounter++;
            }
            else if(SaveManager.PlayerData.status == 2) { //Corrupted or some other random file renamed and placed in saves folder
                _columns[0].Rows[i].text = "Error";
                _columns[0].Rows[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "UNABLE";
                _columns[0].Rows[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "TO READ";
            }
        }
        if(emptySaveCounter == 3) {
            GameEvents.Instance.StopMusic.Invoke();
            SaveManager.PlayerData.saveNum = 0;
            StartIntro();
        }
    }

    //****************************//
    //                            //
    //      MENU NAVIGATION       //
    //                            //
    //****************************//

    public override void OnCloseMenu() {
        //don't allow main menu to be closed...
    }

    public override void OnReturn(InputValue value) {
        SaveManager.Instance.Erase(_currentRow);
        RefreshSaves();
    }


    // Handles when an option is selected
    public override void OnSubmit() {
        GameEvents.Instance.LoadSave.Invoke(_currentRow);
        switch (SaveManager.PlayerData.status) {
            case 0:
                GameManager.Instance.LoadGame();
                GameEvents.Instance.MajorEvent.Invoke(MajorEvent.ui_closed);
                break;
            case 1:
                StartIntro();
                SaveManager.PlayerData.saveNum = _currentRow;
                SaveManager.PlayerData.TylerValue = UnityEngine.Random.Range(0,100);
                if (SaveManager.PlayerData.TylerValue == 12 && UnityEngine.Random.Range(0, 100) == 12) StartCoroutine(Piracy());
                break;
        }
    }

    //easter egg
    private IEnumerator Piracy() {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1.2f, 5.6f));
        SceneManager.LoadScene("piracy");
    }

    //****************************//
    //                            //
    //   INTRO CUTSCENE-RELATED   //
    //                            //
    //****************************//

    // Handles playing the intro cutscene when a new game is started
    public void StartIntro() {
        GameEvents.Instance.StopMusic.Invoke();
        _canvas.enabled = false;
        _playerInput.SwitchCurrentActionMap("Cutscene");
        GameEvents.Instance.MajorEvent.Invoke(MajorEvent.cutscene_started);
        _introCutsceneCanvas.SetActive(true);
        _introCutsceneTimeline.SetActive(true);
    }

    // Cleans up intro cutscene
    public void EndCutscene() {
        //_introCutsceneCanvas.SetActive(false);
        //_introCutsceneTimeline.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameEvents.Instance.MajorEvent.Invoke(MajorEvent.cutscene_ended);
    }

    //Activate main menu, instead of a cutscene or anything
    private void GoToMainMenu() {
        GameEvents.Instance.MajorEvent.Invoke(MajorEvent.ui_opened);
        _canvas.enabled = true;
        _playerInput.SwitchCurrentActionMap("UI");
        GameEvents.Instance.PlaySong.Invoke(_mainMenuSong);
        RefreshSaves();
    }
}

