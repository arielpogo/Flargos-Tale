using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NavigatableMenu : MonoBehaviour{
    [Serializable]
    protected class ColumnInfo {
        [SerializeField] public TextMeshProUGUI[] Rows;
    }

    [SerializeField] protected ColumnInfo[] _columns; //jagged array of all of the selectables options (buttons) in the overworld menu.
                                                    //_columns[c] <- select a column in a page
                                                    //_columns[c].Rows[r] <- select a row (an actual TextMeshProUGui)

    protected Vector2 _navDirection;
    protected int _currentColumn = -1;
    protected int _currentRow = -1;

    [SerializeField] protected NavigatableMenu _previousMenu = null; //what menu to return control to afterward

    protected Color _colorIdle = Color.white;
    protected Color _colorHighlight = new(1, 1, 0, 1);

    public void OnNavigate(InputAction.CallbackContext context) {
        if (context.performed) {
            _navDirection = context.ReadValue<Vector2>();
            _navDirection.x = Math.Sign(_navDirection.x); //unit vector
            _navDirection.y = Math.Sign(_navDirection.y);

            //fancy wraparound selection, when nothing is selected
            if (_currentColumn == -1 || _currentRow == -1) {
                if (_navDirection.x > 0) { //if you press D, current selection is set to top left
                    _currentRow = 0;
                    _currentColumn = 0;
                }
                else if (_navDirection.x < 0) {//if you press A, current selection is set to top right
                    _currentRow = 0;
                    _currentColumn = _columns.Length - 1;
                }
                else if (_navDirection.y > 0) { //if you press W, then the current selection is set to the bottom left
                    _currentColumn = 0;
                    _currentRow = _columns[_currentColumn].Rows.Length - 1;
                }
                else if (_navDirection.y < 0) { //if you press S, then the current selection is set to the top left
                    _currentColumn = 0;
                    _currentRow = 0;
                }
            }
            else {
                _columns[_currentColumn].Rows[_currentRow].color = _colorIdle; //decolor, in case we do move
                //changing column/row
                if (_navDirection.x != 0) _currentColumn += (int)_navDirection.x;

                if (_navDirection.y != 0) _currentRow -= (int)_navDirection.y; //minus because the input vector is increasing upwards, while the list goes from top to bottom (increasing index downwards)

                //wrap around
                if (_currentColumn > _columns.Length - 1) _currentColumn = 0; //go beyond right, go to left
                else if (_currentColumn < 0) _currentColumn = _columns.Length - 1; //go beyond left, go to right
                if (_currentRow > _columns[_currentColumn].Rows.Length - 1) _currentRow = 0; //go beyond bottom, go to top
                else if (_currentRow < 0) _currentRow = _columns[_currentColumn].Rows.Length - 1; //go beyond top, go to bottom
            }
            _columns[_currentColumn].Rows[_currentRow].color = _colorHighlight;
        }
    }


    public virtual void OnCloseMenu(InputAction.CallbackContext context) {

    }

    public virtual void OnReturn(InputAction.CallbackContext context) {

    }
    public virtual void OnSelect(InputAction.CallbackContext context) {
        
    }

}
