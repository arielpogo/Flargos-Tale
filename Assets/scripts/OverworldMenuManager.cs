using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldMenu : NavigatableMenu{
    [SerializeField] private TextMeshProUGUI[] _menus = new TextMeshProUGUI[3];

    public OverworldMenu(NavigatableMenu menuCalling) {
        GameManager.Instance.ChangeGameState(GameState.overworldMenu);
        _previousMenu = menuCalling;
    }

    /**public override void OnCloseMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            if(_currentColumn != -1 && _currentRow != -1) _columns[_currentColumn].Rows[_currentRow].color = _colorIdle;
            _currentRow = -1;
            _currentColumn = -1;
            for (int i = 0; i < _menus.Length; i++) _menus[i].SetActive(false);
            GameManager.Instance.ChangeGameState(GameState.overworld);
        }
    }

    public override void OnReturn(InputAction.CallbackContext context) {
        if (context.performed) {
            if (_currentColumn != -1 && _currentRow != -1) _columns[_currentColumn].Rows[_currentRow].color = _colorIdle;
            _currentRow = -1;
            _currentColumn = -1;
            _menus[_currentPage].SetActive(false);
            if(_currentPage == 0) GameManager.Instance.ChangeGameState(GameState.overworld); //in case the player presses Q when they're already on the general menu
            _currentPage = 0;
        }
    }
    public override void OnSelect(InputAction.CallbackContext context) {
        if (context.performed) {
            switch (_currentPage) {
                case 0: //general menu
                    _columns[_currentColumn].Rows[_currentRow].color = _colorIdle;
                    _currentPage = _currentRow + 1;
                    _menus[_currentRow + 1].SetActive(true);
                    _currentColumn = -1;
                    _currentRow = -1;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }
    }**/
}
