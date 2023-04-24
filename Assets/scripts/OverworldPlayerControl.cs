using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

//Handles movement for the player
public class PlayerControl : MonoBehaviour {

    //****************************//
    //                            //
    //         VARIABLES          //
    //                            //
    //****************************//

    //movement-reated
    [SerializeField] private float _moveSpeed = 1f; //how fast the player moves, unity editor overrides
    [SerializeField] private float _collisionOffset = 0.05f; //how far you can be from any collisions, unity editor overrides
    [SerializeField] private float _interactDistance = 2.0f;
    [SerializeField] private ContactFilter2D _movementFilter; //The settings that determine where a collision can occur, see unity inspector

    private Vector2 _movementInput; //updated by OnMove()
    private Vector2 _overworldLookDirection = new(0,-1); //updated by FixedUpdate(), represents the vector the player appears to be facing

    private List<RaycastHit2D> _castCollisions = new(); //list of collisions, updated in AttemptMove()

    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private PlayerInput _playerInput;

    private int _primaryDir = 2; //1 = a or d, 2 = w or s

    //overworld menu navigation-related
    private Color _colorIdle = Color.white;
    private Color _colorHighlight = new(1, 1, 0, 1);

    [SerializeField] private GameObject _generalMenu;
    [SerializeField] private GameObject _questMenu;
    [SerializeField] private GameObject _statusMenu;
    [SerializeField] private GameObject _inventoryMenu;

    //4 is the hard-coded amount of pages. (First overworld menu, item page, quest page, stats page)
    //Each page must be a square/rectangle of R x C size, R is the number of Rows, C is Columns
    private const int _NUM_PAGES = 4;
    private TextMeshProUGUI[][][] _overworldMenuOptions; //field 1 is the page, field 2 is the column, field 3 is the row

    [Serializable]
    private class PageInfo {
        [SerializeField] public ColumnInfo[] Columns;
    }

    [Serializable]
    private class ColumnInfo {
        [SerializeField] public TextMeshProUGUI[] Rows;
    }

    [SerializeField] private PageInfo[] pagesArray;

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

    // Grabbing components, subscribing to events, calculating values.
    private void Start() {
        _rigidBody = GetComponent<Rigidbody2D>(); //sets rb to the Rigidbody2D component in the Player object
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerInput = GetComponent<PlayerInput>();

        for(int p = 0; p < pagesArray.Length; p++) {
            for(int c = 0; c < pagesArray[p].Columns.Length; c++) {
                for(int r = 0; r < pagesArray[p].Columns[c].Rows.Length; r++) {
                    _overworldMenuOptions[p][c][r] = pagesArray[p].Columns[c].Rows[r];
                }
            }
        }

        GameEvents.Instance.OnGameStateChange += UpdateActionMap;
    }
    private void OnDestroy() {
        if(GameEvents.Instance != null) GameEvents.Instance.OnGameStateChange -= UpdateActionMap;
    }

    //****************************//
    //                            //
    //           INPUT            //
    //                            //
    //****************************//

    public void OnMove(InputAction.CallbackContext context) {
        if (context.performed) _movementInput = context.ReadValue<Vector2>(); //the Player Input component will invoke and pass WASD/arrow key keypresses
        else _movementInput = Vector2.zero; 
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.performed){
            //raycast in the direction the sprite is looking, filtering for Raycast Interactables
            RaycastHit2D results = Physics2D.Raycast(_rigidBody.position, _overworldLookDirection, _interactDistance, LayerMask.GetMask("Raycast Interactable"));
            if (results.transform == null) return;
            results.transform.GetComponent<BaseInteractableClass>().Interact();
        }
    }

    public void OnNavigate(InputAction.CallbackContext context) {
        if (context.performed) {
            _navDirection = context.ReadValue<Vector2>();
            _navDirection.x = Math.Sign(_navDirection.x); //unit vector
            _navDirection.y = Math.Sign(_navDirection.y);

            //fancy wraparound selection, when nothing is selected
            if (_currentRow == -1) {
                if (_navDirection.y > 0) { //if you press W, then the current selection is set to the bottom left
                    _currentColumn = 0;
                    _currentRow = _overworldMenuOptions[_currentPage][_currentColumn].Length - 1;
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
                    _currentColumn = _overworldMenuOptions[_currentPage].Length - 1;
                }
            }
            else {
                //would adding the x input to the current column selected be in the limit of columns? If yes, add them, if no, don't.
                if (0 < (_navDirection.x + _currentColumn) && (_navDirection.x + _currentColumn) < _overworldMenuOptions[_currentPage].Length) {
                    _previousColumn = _currentColumn;
                    _currentColumn += (int)_navDirection.x;
                }
                //ditto, but rows
                if (0 < (_navDirection.y + _currentRow) && (_navDirection.y + _currentRow) < _overworldMenuOptions[_currentPage][_currentColumn].Length) {
                    _previousRow = _currentRow;
                    _currentRow += (int)_navDirection.y;
                }
            }

            _overworldMenuOptions[_currentPage][_previousColumn][_previousRow].color = _colorIdle;
            _overworldMenuOptions[_currentPage][_currentColumn][_currentRow].color = _colorHighlight;

        }
    }

    //This calls the GameManager and then UpdateGameState acts upon a call of its delegate, I did this in case in the future I have other systems which rely on knowing the overworld menu was opened
    public void OpenMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            GameManager.Instance.ChangeGameState(GameState.overworldMenu);
            _generalMenu.SetActive(true);
        }
    }

    public void OnCloseMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            _overworldMenuOptions[_currentPage][_currentColumn][_currentRow].color = _colorIdle;
            _currentRow = -1;
            _currentColumn = -1;
            _previousRow = 0;
            _previousColumn = 0;
            _generalMenu.SetActive(false);
            GameManager.Instance.ChangeGameState(GameState.overworld);
        }
    }

    public void OnSelect(InputAction.CallbackContext context) {
        if (context.performed) {
            switch (_currentColumn) {
                case 0:
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

    //****************************//
    //                            //
    //     MOVEMENT FUNCTIONS     //
    //                            //
    //****************************//

    // This will perform the frame-by-frame movement for the player, and sprite logic.
    private void FixedUpdate() {
        if (_movementInput != Vector2.zero) { //if there is input...
            bool moveSuccess = AttemptMove(_movementInput); //check if moved

            if (moveSuccess) _animator.SetBool("isMoving", moveSuccess);
            else { //sliding mechanics, if you can't move diagonally
                if (_movementInput.x != 0) { //...and a/d is being pressed, try moving in x
                    moveSuccess = AttemptMove(new Vector2(_movementInput.x, 0));
                    _animator.SetBool("isMoving", moveSuccess);
                }
                if (!moveSuccess && _movementInput.y != 0) { //if that didn't work, and w/s is being pressed, try moving in y
                    _animator.SetBool("isMoving", AttemptMove(new Vector2(0, _movementInput.y)));
                }
            }

            if (_primaryDir == 1 && Math.Abs(_movementInput.x) < Math.Abs(_movementInput.y)) _primaryDir = 2;  //if x is supposed main direction, but x key isn't held, y new direction
            else if (_primaryDir == 2 && Math.Abs(_movementInput.y) < Math.Abs(_movementInput.x)) _primaryDir = 1;  //vise versa

            //because we have separate y direction sprites (facing forward, backward), we do direction logic in animation editor
            _animator.SetFloat("yDir", _movementInput.y);

            //and because we have only one x direction sprite, we do flipping logic here
            if ((_primaryDir == 1 && _movementInput.x > 0) || (_primaryDir != 1)) {
                _spriteRenderer.flipX = false;
            }
            else if (_primaryDir == 1 && _movementInput.x < 0) {
                _spriteRenderer.flipX = true;
            }

            switch (_primaryDir) {
                case 1:
                    _overworldLookDirection.x = Math.Sign(_movementInput.x); //this prevents getting a smaller x when moving diagonally/up against a wall
                    _overworldLookDirection.y = 0;
                    break;
                case 2:
                    _overworldLookDirection.y = Math.Sign(_movementInput.y);
                    _overworldLookDirection.x = 0;
                    break;
            }
            _animator.SetInteger("primaryDir", _primaryDir);
        }
        else {
            _animator.SetBool("isMoving", false);
            _animator.SetFloat("yDir", _movementInput.y);
        }
        //.DrawRay(_rigidBody.position, _overworldLookDirection * _interactDistance);
    }

    /// <summary>
    /// This function handles the raycasting related to moving (especially diagonally into a wall)
    /// </summary>
    /// <param name="direction">The desired direction to be checked</param>
    /// <returns> Whether you can move in that direction or not</returns>
    private bool AttemptMove(Vector2 direction) {
        //counts potential collisions        
        int count = _rigidBody.Cast(
                        direction, //X and Y values between -1 and 1 that represent direction from the player for collisions
                        _movementFilter, //The settings that determine where a collision can occur
                        _castCollisions, //List to store found collisions after the cast is finished
                        _moveSpeed * Time.fixedDeltaTime + _collisionOffset); //The amount to cast equal to the movement plus an offset

        if (count == 0) { //if 0 collisions, move
            _rigidBody.MovePosition(_rigidBody.position + _moveSpeed * Time.fixedDeltaTime * direction); //add to the current position: the movement input * desired player speed * time = vector
            return true;
        }
        else return false;
    }

    //****************************//
    //                            //
    //            MISC            //
    //                            //
    //****************************//

    private void UpdateActionMap(GameState NewGameState) {
        switch (NewGameState) {
            case GameState.overworld:
            case GameState.cutscene_with_control:
                _playerInput.SwitchCurrentActionMap("Overworld");
                break;
            case GameState.overworldMenu:
                _playerInput.SwitchCurrentActionMap("UI");
                break;
            case GameState.dialogue:
                _playerInput.SwitchCurrentActionMap("Dialogue");
                break;
            case GameState.cutscene:
                _playerInput.SwitchCurrentActionMap("Cutscene");
                break;

        }
    }
}
