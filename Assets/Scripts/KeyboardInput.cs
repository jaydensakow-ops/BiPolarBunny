using UnityEngine;
using UnityEngine.InputSystem;

// This script handles all keyboard input for the player
// It uses WASD keys for movement and other keys for actions
//
// SETUP: Drag this component onto your player object.
//        In the Inspector, drag your player's PlayerMovement component into the
//        "Player Movement" slot below.  And the PlayerCombat component into
//        its slot.  That's it!.
public class KeyboardInput : MonoBehaviour
{
    // Drag the PlayerMovement component here in the Inspector
    public PlayerMovement PlayerMovement;

    // Drag the PlayerCombat component here in the Inspector
    public PlayerCombat player;

    private void Update()
    {
        // If no PlayerMovement is wired up, skip everything
        if (PlayerMovement == null)
        {
            return;
        }

        // Read the movement direction from WASD keys
        Vector2 movement = GetMovement();

        // Tell PlayerMovement to move in that direction
        // Always call Move(), even when movement is zero. This is what stops the player
        // when no input is pressed. If we skip Move() on zero, the player keeps drifting.
        PlayerMovement.Move(movement, this);

        // If the attack key (Space) was pressed this frame, trigger an attack
        if (WasAttackButtonPressed())
        {
            player.Attack();
        }
    }

    // Gets the movement input from the keyboard as a Vector2 (x and y direction)
    public Vector2 GetMovement()
    {
        // If no keyboard is available, return no movement
        if (IsKeyboardUnavailable())
        {
            return Vector2.zero;
        }

        // Get horizontal movement from A and D keys (-1 for left, 1 for right, 0 for neither)
        float horizontalInput = GetHorizontalInput();
        
        // Get vertical movement from W and S keys (1 for up, -1 for down, 0 for neither)
        float verticalInput = GetVerticalInput();

        // Combine horizontal and vertical into a Vector2
        // For example: pressing D and W gives Vector2(1, 1) - moving right and up
        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        
        // Normalize diagonal movement so the player doesn't move faster diagonally
        movement = AdjustForDiagonalMovement(movement);
        
        // Return the final movement vector
        return movement;
    }

    // Checks if the place button (E key) was pressed
    public bool WasPlaceButtonPressed()
    {
        // If no keyboard is available, return false
        if (IsKeyboardUnavailable())
        {
            return false;
        }
        
        // Check if E key was pressed THIS frame (not held from previous frames)
        // wasPressedThisFrame only returns true once per key press
        return Keyboard.current[GameParameters.PlaceKey].wasPressedThisFrame;
    }

    // Checks if the pickup button (F key) was pressed
    public bool WasPickupButtonPressed()
    {
        // If no keyboard is available, return false
        if (IsKeyboardUnavailable())
        {
            return false;
        }
        
        // Check if F key was pressed THIS frame (not held from previous frames)
        // wasPressedThisFrame only returns true once per key press
        return Keyboard.current[GameParameters.PickupKey].wasPressedThisFrame;
    }

    // Checks if the attack button (Space key) was pressed
    public bool WasAttackButtonPressed()
    {
        // If no keyboard is available, return false
        if (IsKeyboardUnavailable())
        {
            return false;
        }
        
        // Check if Space key was pressed THIS frame
        return Keyboard.current[GameParameters.AttackKey].wasPressedThisFrame;
    }

    // Checks if the keyboard is unavailable (not connected or not working)
    private bool IsKeyboardUnavailable()
    {
        // Keyboard.current is null if no keyboard is detected
        // This can happen on some platforms or in certain situations
        return Keyboard.current == null;
    }

    // Gets horizontal input from A and D keys
    // Returns -1 for left (A), 1 for right (D), or 0 for neither/both
    private float GetHorizontalInput()
    {
        // Start with no horizontal movement
        float horizontal = 0f;
        
        // If A key (left) is pressed, subtract 1 (move left)
        if (IsLeftKeyPressed())
        {
            horizontal = horizontal - 1f;
        }
        
        // If D key (right) is pressed, add 1 (move right)
        if (IsRightKeyPressed())
        {
            horizontal = horizontal + 1f;
        }
        
        // Return the final horizontal value
        // Note: If both A and D are pressed, they cancel out to 0
        return horizontal;
    }

    // Gets vertical input from W and S keys
    // Returns 1 for up (W), -1 for down (S), or 0 for neither/both
    private float GetVerticalInput()
    {
        // Start with no vertical movement
        float vertical = 0f;
        
        // If W key (up) is pressed, add 1 (move up)
        if (IsUpKeyPressed())
        {
            vertical = vertical + 1f;
        }
        
        // If S key (down) is pressed, subtract 1 (move down)
        if (IsDownKeyPressed())
        {
            vertical = vertical - 1f;
        }
        
        // Return the final vertical value
        // Note: If both W and S are pressed, they cancel out to 0
        return vertical;
    }

    private bool IsLeftKeyPressed()
    {
        return Keyboard.current[GameParameters.MoveLeft].isPressed;
    }

    private bool IsRightKeyPressed()
    {
        return Keyboard.current[GameParameters.MoveRight].isPressed;
    }

    private bool IsUpKeyPressed()
    {
        return Keyboard.current[GameParameters.MoveUp].isPressed;
    }

    private bool IsDownKeyPressed()
    {
        return Keyboard.current[GameParameters.MoveDown].isPressed;
    }

    // Normalizes diagonal movement so the player doesn't move faster diagonally
    private Vector2 AdjustForDiagonalMovement(Vector2 movement)
    {
        // Check if the movement is diagonal (both x and y are non-zero)
        if (IsDiagonalMovement(movement))
        {
            // Normalize makes the vector length = 1.0
            // Without this, diagonal movement would be 1.414x faster (sqrt(1²+1²))
            // For example: Vector2(1, 1) has length 1.414, but after normalize it becomes Vector2(0.707, 0.707) with length 1.0
            movement.Normalize();
        }
        
        // Return the adjusted movement
        return movement;
    }

    // Checks if the movement vector represents diagonal movement
    private bool IsDiagonalMovement(Vector2 movement)
    {
        // sqrMagnitude is the squared length of the vector (x² + y²)
        // For Vector2(1, 1): sqrMagnitude = 1² + 1² = 2, which is > 1
        // For Vector2(1, 0): sqrMagnitude = 1² + 0² = 1, which is not > 1
        // For Vector2(0, 1): sqrMagnitude = 0² + 1² = 1, which is not > 1
        // So this returns true only when moving diagonally
        return movement.sqrMagnitude > 1f;
    }
}