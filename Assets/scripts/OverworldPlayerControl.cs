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
    private Color _colorHighlight = new Color(1, 1, 0, 1);

    public GameObject OverworldMenu;
    public TextMeshProUGUI[] options = new TextMeshProUGUI[3];

    private Vector2 _navDirection;
    private int _selectedOption = -1; // -1 == unselected so far
    private int _maxOption;

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

        _maxOption = options.Length - 1;

        GameEvents.Instance.OnGameStateChange += UpdateActionMap;
    }
    private void OnDestroy() {
        GameEvents.Instance.OnGameStateChange -= UpdateActionMap;
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
            if (_selectedOption == -1) {
                if (_navDirection.y > 0) { _selectedOption = _maxOption; }
                else if (_navDirection.y < 0) { _selectedOption = 0; }
            }
            else {
                if (_navDirection.y < 0) { _selectedOption++; }
                else if (_navDirection.y > 0) { _selectedOption--; }

                if (_selectedOption < 0) { _selectedOption = _maxOption; }
                else if (_selectedOption > _maxOption) { _selectedOption = 0; }
            }

            for (int i = 0; i <= _maxOption; i++) {
                if (i == _selectedOption) { options[i].color = _colorHighlight; }
                else { options[i].color = _colorIdle; }
            }
        }
    }

    //This calls the GameManager and then UpdateGameState acts upon a call of its delegate, I did this in case in the future I have other systems which rely on knowing the overworld menu was opened
    public void OpenMenu(InputAction.CallbackContext context) {
        if (context.performed) GameManager.Instance.ChangeGameState(GameState.overworldMenu);
        OverworldMenu.SetActive(true);
    }

    public void OnCloseMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            for (int i = 0; i <= _maxOption; i++) options[i].color = _colorIdle;
            _selectedOption = -1;
            OverworldMenu.SetActive(false);
            GameManager.Instance.ChangeGameState(GameState.overworld);
        }
    }

    public void OnSelect(InputAction.CallbackContext context) {
        if (context.performed) {
            switch (_selectedOption) {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
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
        Debug.DrawRay(_rigidBody.position, _overworldLookDirection * _interactDistance);
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
