using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Handles movement for the player
public class OverworldPlayerControl : MonoBehaviour {

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

    private Animator _shadowAnimator;
    private SpriteRenderer _shadowSpriteRenderer;
    private Rigidbody2D _shadowRigidBody;
    private Vector2 _shadowOffset = new(0, -.88f);

    private int _primaryDir = 2; //1 = a or d, 2 = w or s

    [SerializeField] private GameObject _generalMenuPrefab;

    //****************************//
    //                            //
    //    START AND ONDESTROY     //
    //                            //
    //****************************//

    // Grabbing components, subscribing to events, calculating values.
    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>(); //sets rb to the Rigidbody2D component in the Player object
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerInput = GetComponent<PlayerInput>();

        _shadowAnimator = GameObject.FindWithTag("PlayerShadow").GetComponent<Animator>();
        _shadowSpriteRenderer = GameObject.FindWithTag("PlayerShadow").GetComponent<SpriteRenderer>();
        _shadowRigidBody = GameObject.FindWithTag("PlayerShadow").GetComponent<Rigidbody2D>();

        GameEvents.Instance.OnGameStateChange += UpdateActionMap;
    }
    private void Start() {
        GameManager.Instance.ChangeGameState(GameState.overworld);
    }

    private void OnDestroy() {
        if (GameEvents.Instance != null) GameEvents.Instance.OnGameStateChange -= UpdateActionMap;
    }

    //****************************//
    //                            //
    //           INPUT            //
    //                            //
    //****************************//

    public void OnMove(InputValue value) {
        _movementInput = value.Get<Vector2>();
    }

    public void OnInteract() {
        //raycast in the direction the sprite is looking, filtering for Raycast Interactables
        RaycastHit2D results = Physics2D.Raycast(_rigidBody.position, _overworldLookDirection, _interactDistance, LayerMask.GetMask("Raycast Interactable"));
        if (results.transform == null) return;
        results.transform.GetComponent<BaseInteractableClass>().Interact();
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

            _shadowAnimator.SetBool("isMoving", _animator.GetBool("isMoving"));

            if (_primaryDir == 1 && Math.Abs(_movementInput.x) < Math.Abs(_movementInput.y)) _primaryDir = 2;  //if x is supposed main direction, but x key isn't held, y new direction
            else if (_primaryDir == 2 && Math.Abs(_movementInput.y) < Math.Abs(_movementInput.x)) _primaryDir = 1;  //vise versa

            //because we have separate y direction sprites (facing forward, backward), we do direction logic in animation editor
            _animator.SetFloat("yDir", _movementInput.y);
            _shadowAnimator.SetFloat("yDir", _movementInput.y);

            //and because we have only one x direction sprite, we do flipping logic here
            if ((_primaryDir == 1 && _movementInput.x > 0) || (_primaryDir != 1)) {
                _spriteRenderer.flipX = false;
                _shadowSpriteRenderer.flipX = false;
            }
            else if (_primaryDir == 1 && _movementInput.x < 0) {
                _spriteRenderer.flipX = true;
                _shadowSpriteRenderer.flipX = true;
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
            _shadowAnimator.SetInteger("primaryDir", _primaryDir);
        }
        else {
            _animator.SetBool("isMoving", false);
            _shadowAnimator.SetBool("isMoving", false);
            _animator.SetFloat("yDir", _movementInput.y);
            _shadowAnimator.SetFloat("yDir", _movementInput.y);
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
            _shadowRigidBody.MovePosition(_rigidBody.position + _moveSpeed * Time.fixedDeltaTime * direction +_shadowOffset);
            return true;
        }
        else return false;
    }

    //****************************//
    //                            //
    //            MISC            //
    //                            //
    //****************************//

    public void OnOpenMenu() {
         Factory.InstantiateNavigableMenu(_generalMenuPrefab, null, GameState.overworld); //previous menu == null because this class isn't a navigatablemenu
    }

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
