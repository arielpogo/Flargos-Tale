using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//Handles movement for the player
public class PlayerControl : MonoBehaviour {
    [SerializeField] private float moveSpeed = 1f; //how fast the player moves, unity editor overrides
    [SerializeField] private float collisionOffset = 0.05f; //how far you can be from any collisions, unity editor overrides
    [SerializeField] private float interactDistance = 2.0f;
    [SerializeField] private ContactFilter2D movementFilter; //The settings that determine where a collision can occur, see unity inspector

    private Vector2 movementInput; //updated by OnMove()
    private Vector2 overworldLookDirection = new(0,-1); //updated by FixedUpdate(), represents the vector the player appears to be facing

    private List<RaycastHit2D> castCollisions = new(); //list of collisions, updated in AttemptMove()

    private Animator animator;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;

    private int primaryDir = 2; //1 = a or d, 2 = w or s
    
    // Start is called before the first frame update
    /// <summary>
    /// Grabbing components.
    /// </summary>
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>(); //sets rb to the Rigidbody2D component in the Player object
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    //recommended for physics-related updating
    /// <summary>
    /// This will perform the frame-by-frame movement for the player, and sprite logic.
    /// </summary>
    private void FixedUpdate() {
        if (movementInput != Vector2.zero) { //if there is input...
            bool moveSuccess = AttemptMove(movementInput); //check if moved

            if (moveSuccess) animator.SetBool("isMoving", moveSuccess);
            else { //sliding mechanics, if you can't move diagonally
                if (movementInput.x != 0) { //...and a/d is being pressed, try moving in x
                    moveSuccess = AttemptMove(new Vector2(movementInput.x, 0));
                    animator.SetBool("isMoving", moveSuccess);
                }
                if (!moveSuccess && movementInput.y != 0) { //if that didn't work, and w/s is being pressed, try moving in y
                    animator.SetBool("isMoving", AttemptMove(new Vector2(0, movementInput.y)));
                }
            }

            if (primaryDir == 1 && Math.Abs(movementInput.x) < Math.Abs(movementInput.y))  primaryDir = 2;  //if x is supposed main direction, but x key isn't held, y new direction
            else if (primaryDir == 2 && Math.Abs(movementInput.y) < Math.Abs(movementInput.x))  primaryDir = 1;  //vise versa

            //because we have separate y direction sprites (facing forward, backward), we do direction logic in animation editor
            animator.SetFloat("yDir", movementInput.y);

            //and because we have only one x direction sprite, we do flipping logic here
            if ((primaryDir == 1 && movementInput.x > 0) || (primaryDir != 1)) {
                spriteRenderer.flipX = false;
            }
            else if (primaryDir == 1 && movementInput.x < 0) {
                spriteRenderer.flipX = true;
            }

            switch (primaryDir) {
                case 1:
                    overworldLookDirection.x = Math.Sign(movementInput.x); //this prevents getting a smaller x when moving diagonally/up against a wall
                    overworldLookDirection.y = 0;
                    break;
                case 2:
                    overworldLookDirection.y = Math.Sign(movementInput.y);
                    overworldLookDirection.x = 0;
                    break;
            }
            animator.SetInteger("primaryDir", primaryDir);
        }
        else{
            animator.SetBool("isMoving", false);
            animator.SetFloat("yDir", movementInput.y);
        }
        Debug.DrawRay(rigidBody.position, overworldLookDirection * interactDistance);
    }

    /// <summary>
    /// This function handles the raycasting related to moving (especially diagonally into a wall)
    /// </summary>
    /// <param name="direction">The desired direction to be checked</param>
    /// <returns> Whether you can move in that direction or not</returns>
    private bool AttemptMove(Vector2 direction){
        //counts potential collisions        
        int count = rigidBody.Cast(
                        direction, //X and Y values between -1 and 1 that represent direction from the player for collisions
                        movementFilter, //The settings that determine where a collision can occur
                        castCollisions, //List to store found collisions after the cast is finished
                        moveSpeed * Time.fixedDeltaTime + collisionOffset); //The amount to cast equal to the movement plus an offset

        if (count == 0) { //if 0 collisions, move
            rigidBody.MovePosition(rigidBody.position + moveSpeed * Time.fixedDeltaTime * direction); //add to the current position: the movement input * desired player speed * time = vector
            return true;
        }
        else return false;
    }

    /////////////////////////////////////////////INPUT//////////////////////////////////////////////////////////

    /// <summary>
    /// This is what the Unity Input System will call when WASD/Joystick input is given, it sets movementinput to the vector of movement
    /// </summary>
    /// <param name="context">Default parameter when dealing with the Unity Input System</param>
    public void OnMove(InputAction.CallbackContext context) {
        if (context.performed) movementInput = context.ReadValue<Vector2>(); //the Player Input component will invoke and pass WASD/arrow key keypresses
        else movementInput = Vector2.zero; 
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.performed){
            //raycast in the direction the sprite is looking, filtering for Raycast Interactables
            RaycastHit2D results = Physics2D.Raycast(rigidBody.position, overworldLookDirection, interactDistance, LayerMask.GetMask("Raycast Interactable"));
            if (results.transform == null) return;
            results.transform.GetComponent<BaseInteractableClass>().Interact();
            
        }
    }

    /// <summary>
    /// Handles what happens when the OverworldMenu is opened by changing the gamestate
    /// </summary>
    /// <param name="context">Default parameter when dealing with the Unity Input System</param>
    public void OpenMenu(InputAction.CallbackContext context) {
        if (context.performed) GameManager.Instance.ChangeGameState(GameState.overworldMenu);
    }
}
