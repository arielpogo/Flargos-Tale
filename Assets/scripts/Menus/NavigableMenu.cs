using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// General class for a 2D menu of options. Each derivative class handles further functionality (i.e. InventoryMenu handles getting the inventory, not NavigableMenu).
/// When another menu calls it, that menu should be passed and then deactivated by the called menu. Then, when returning, the called menu should be reactivated and then deactivate the caller menu.
/// Only the base menu changes the GameState, the base menu has a PreviousMenu of null, like a linked list.
/// </summary>
public abstract class NavigableMenu : MonoBehaviour {
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

    protected NavigableMenu _previousMenu = null; //what menu to return control to afterward, set by the calling menu
    protected GameState _returnGameState;

    protected Color _colorIdle = Color.white;
    protected Color _colorHighlight = new(1, 1, 0, 1);

    /// <summary>
    /// Called by the Factory.InstantiateNavigableMenu() function
    /// </summary>
    public void Setup(NavigableMenu previousMenu, GameState returnGameState) {
        _previousMenu = previousMenu;
        _returnGameState = returnGameState;
    }

    /// <summary>
    /// When WASD is pressed, what to do?
    /// </summary>
    public virtual void OnNavigate(InputValue value) {
        Debug.Log("On navigate");
        if (enabled) {
            _navDirection = value.Get<Vector2>();
            _navDirection.x = Math.Sign(_navDirection.x); //unit vector
            _navDirection.y = Math.Sign(_navDirection.y);

            //fancy wraparound selection, when nothing is selected
            if (_currentColumn == -1 || _currentRow == -1) {
                if (_navDirection.y > 0) { //if you press W, then the current selection is set to the bottom left
                    _currentColumn = 0;
                    _currentRow = _columns[_currentColumn].Rows.Length - 1;
                }
                else if (_navDirection.y < 0) { //if you press S, then the current selection is set to the top left
                    _currentColumn = 0;
                    _currentRow = 0;
                }
                else if (_navDirection.x > 0) { //if you press D, current selection is set to top left
                    _currentRow = 0;
                    _currentColumn = 0;
                }
                else if (_navDirection.x < 0) {//if you press A, current selection is set to top right
                    _currentRow = 0;
                    _currentColumn = _columns.Length - 1;
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

            //Debug.Log(String.Format("Row: {0}, Column: {1}, Name: {2}", _currentRow.ToString(), _currentColumn.ToString(), gameObject.name));
        }
    }

    /// <summary>
    /// When Tab is pressed, what to do?
    /// </summary>
    public virtual void OnCloseMenu() {
        TotalMenuClose();
    }

    //for when we want the whole menu to close, subsequently calling all the way down
    public virtual void TotalMenuClose() {
        if (_previousMenu == null) {
            GameManager.Instance.ChangeGameState(_returnGameState);
            Destroy(gameObject);
        }
        else {
            _previousMenu.enabled = true;
            _previousMenu.TotalMenuClose();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// When Q is pressed, what to do?
    /// </summary>
    public virtual void OnReturn(InputValue value) {
        InputAction.CallbackContext context = value.GetCallBackContext();
        if (context.performed && enabled) {
            if (_previousMenu == null) {
                GameManager.Instance.ChangeGameState(_returnGameState);
                Destroy(gameObject);
            }
            else {
                _previousMenu.enabled = true;
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// When E is pressed, what to do?
    /// </summary>
    public virtual void OnSubmit() {

    }
}
