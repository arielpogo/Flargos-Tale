using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class OverworldMenuManager : MonoBehaviour{
    //overworld menu navigation-related
    private Color _colorIdle = Color.white;
    private Color _colorHighlight = new(1, 1, 0, 1);

    [SerializeField] private GameObject[] _menus = new GameObject[4];

    [Serializable]
    private class PageInfo {
        [SerializeField] public ColumnInfo[] Columns;
    }

    [Serializable]
    private class ColumnInfo {
        [SerializeField] public TextMeshProUGUI[] Rows;
    }

    [SerializeField] private PageInfo[] _overworldMenuPages; //jagged array of all of the selectables options (buttons) in the overworld menu.
                                                             //_overworldMenuPages[p] <- select a page
                                                             //_overworldMenuPages[p].Columns[c] <- select a column in a page
                                                             //_overworldMenuPages[p].Columns[c].Rows[r] <- select a row (an actual TextMeshProUGui)

    private Vector2 _navDirection;
    private int _currentPage = 0;
    private int _previousColumn = 0; //this is so when selecting an option for the first time, it doesn't try reverting the color of the previous, "-1th" option
    private int _currentColumn = -1;
    private int _previousRow = 0; //ditto
    private int _currentRow = -1;

    //****************************//
    //                            //
    //    START AND ONDESTROY     //
    //                            //
    //****************************//

    public void OnNavigate(InputAction.CallbackContext context) {
        if (context.performed) {
            _navDirection = context.ReadValue<Vector2>();
            _navDirection.x = Math.Sign(_navDirection.x); //unit vector
            _navDirection.y = Math.Sign(_navDirection.y);

            //fancy wraparound selection, when nothing is selected
            if (_currentRow == -1) {
                if (_navDirection.y > 0) { //if you press W, then the current selection is set to the bottom left
                    _currentColumn = 0;
                    _currentRow = _overworldMenuPages[_currentPage].Columns[_currentColumn].Rows.Length - 1;
                }
                else if (_navDirection.y < 0) { //if you press S, then the current selection is set to the top left
                    _currentColumn = 0;
                    _currentRow = 0;
                }
            }
            else if (_currentColumn == -1) {
                if (_navDirection.x > 0) { //if you press D, current selection is set to top left
                    _currentRow = 0;
                    _currentColumn = 0;
                }
                else if (_navDirection.x < 0) {//if you press A, current selection is set to top right
                    _currentRow = 0;
                    _currentColumn = _overworldMenuPages[_currentPage].Columns.Length - 1;
                }
            }
            else {
                //changing column/row
                if (_navDirection.x != 0) {
                    _previousColumn = _currentColumn;
                    _currentColumn += (int)_navDirection.x;
                }
                if (_navDirection.y != 0) {
                    _previousRow = _currentRow;
                    _currentRow -= (int)_navDirection.y; //minus because the input vector is increasing upwards, while the list goes from top to bottom (increasing index downwards)
                }

                //wrap around
                if (_currentRow > _overworldMenuPages[_currentPage].Columns[_currentColumn].Rows.Length - 1) _currentRow = 0; //go beyond bottom, go to top
                else if (_currentRow < 0) _currentRow = _overworldMenuPages[_currentPage].Columns[_currentColumn].Rows.Length - 1; //go beyond top, go to bottom
                if (_currentColumn > _overworldMenuPages[_currentPage].Columns.Length - 1) _currentColumn = 0; //go beyond right, go to left
                else if (_currentColumn < 0) _currentColumn = _overworldMenuPages[_currentPage].Columns.Length - 1; //go beyond left, go to right
            }

            _overworldMenuPages[_currentPage].Columns[_previousColumn].Rows[_previousRow].color = _colorIdle;
            _overworldMenuPages[_currentPage].Columns[_currentColumn].Rows[_currentRow].color = _colorHighlight;
        }
    }

    //This calls the GameManager and then UpdateGameState acts upon a call of its delegate, I did this in case in the future I have other systems which rely on knowing the overworld menu was opened
    public void OpenMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            GameManager.Instance.ChangeGameState(GameState.overworldMenu);
            _menus[0].SetActive(true);
        }
    }

    public void OnCloseMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            _overworldMenuPages[_currentPage].Columns[_currentColumn].Rows[_currentRow].color = _colorIdle;
            _currentRow = -1;
            _currentColumn = -1;
            _previousRow = 0;
            _previousColumn = 0;
            for (int i = 0; i < _menus.Length; i++) _menus[i].SetActive(false);
            GameManager.Instance.ChangeGameState(GameState.overworld);
        }
    }

    public void OnSelect(InputAction.CallbackContext context) {
        if (context.performed) {
            switch (_currentPage) {
                case 0: //general menu
                    _currentPage = _currentRow + 1;
                    _menus[_currentRow + 1].SetActive(true);
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }
    }
}
