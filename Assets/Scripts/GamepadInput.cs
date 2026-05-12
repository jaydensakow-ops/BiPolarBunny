using UnityEngine;
using UnityEngine.InputSystem;

// This script handles all gamepad/controller input for the player
// It supports analog stick, D-pad, and button inputs
//
// SETUP: Drag this component onto your player object.
//        In the Inspector, drag your player's PlayerMovement component into the
//        "Player Movement" slot below. And the PlayerCombat component into
//        its slot.  That's it!
public class GamepadInput : MonoBehaviour
{
    // Drag the PlayerMovement component here in the Inspector
    public PlayerMovement PlayerMovement;

    // Drag the PlayerCombat component here in the Inspector
    public PlayerCombat PlayerCombat;

    // The minimum stick movement required to register as input (prevents stick drift)
    // 0.2 means the stick must move at least 20% from center before it counts
    public float Deadzone = 0.2f;
    
    // Which gamepad to use (0 = first controller, 1 = second controller, etc.)
    public int GamepadIndex = 0;
    
    // Whether to check D-pad input in addition to analog stick
    // If true, D-pad will override analog stick when pressed
    public bool UseDpad = true;

    private void Update()
    {
        // If no PlayerMovement is wired up, skip everything
        if (PlayerMovement == null)
        {
            return;
        }

        // Read the movement direction from the gamepad
        Vector2 movement = GetMovement();

        // Tell PlayerMovement to move in that direction
        // Always call Move(), even when movement is zero — this is what stops the player
        // when no input is pressed. If we skip Move() on zero, the player keeps drifting.
        PlayerMovement.Move(movement, this);

        // If the attack button (West button - X on Xbox, Square on PlayStation) was pressed, trigger an attack
        if (WasAttackButtonPressed())
        {
            PlayerCombat.Attack();
        }
    }

    // Gets the movement input from the gamepad as a Vector2 (x and y direction)
    public Vector2 GetMovement()
    {
        // Get the gamepad object we want to read from
        Gamepad gamepad = GetGamepad();
        
        // If no gamepad is available, return no movement
        if (gamepad == null)
        {
            return Vector2.zero;
        }

        // Get movement from the left analog stick
        Vector2 movement = GetStickMovement(gamepad);
        
        // If D-pad is enabled, check if it's being used and override stick input
        if (UseDpad)
        {
            movement = GetDpadMovementIfActive(gamepad, movement);
        }
        
        // Normalize diagonal movement so the player doesn't move faster diagonally
        movement = AdjustForDiagonalMovement(movement);
        
        // Return the final movement vector
        return movement;
    }

    // Checks if the place button (South button - A on Xbox, X on PlayStation) was pressed
    public bool WasPlaceButtonPressed()
    {
        // If no gamepad is connected, return false
        if (Gamepad.current == null)
        {
            return false;
        }
        
        // Check if the south button was pressed THIS frame (not held from previous frames)
        // wasPressedThisFrame only returns true once per button press
        return Gamepad.current.buttonSouth.wasPressedThisFrame;
    }

    // Checks if the pickup button (North button - Y on Xbox, Triangle on PlayStation) was pressed
    public bool WasPickupButtonPressed()
    {
        // If no gamepad is connected, return false
        if (Gamepad.current == null)
        {
            return false;
        }
        
        // Check if the north button was pressed THIS frame
        return Gamepad.current.buttonNorth.wasPressedThisFrame;
    }
    
    // Checks if the attack button (West button - X on Xbox, Square on PlayStation) was pressed
    public bool WasAttackButtonPressed()
    {
        // If no gamepad is connected, return false
        if (Gamepad.current == null)
        {
            return false;
        }
        
        // Check if the west button was pressed THIS frame
        return Gamepad.current.buttonWest.wasPressedThisFrame;
    }

    // Gets the gamepad object at the specified GamepadIndex
    private Gamepad GetGamepad()
    {
        // If no gamepads are connected at all, return null
        if (HasNoGamepads())
        {
            return null;
        }
        
        // If the GamepadIndex is out of range (negative or too high), return null
        if (IsInvalidGamepadIndex())
        {
            return null;
        }
        
        // Return the gamepad at the specified index from the list of all connected gamepads
        return Gamepad.all[GamepadIndex];
    }

    // Checks if there are no gamepads connected
    private bool HasNoGamepads()
    {
        // Gamepad.all is a list of all connected gamepads
        // If Count is 0, no gamepads are connected
        return Gamepad.all.Count == 0;
    }

    // Checks if the GamepadIndex is invalid (out of range)
    private bool IsInvalidGamepadIndex()
    {
        // Index is invalid if it's negative OR if it's >= the number of connected gamepads
        // For example, if 2 gamepads are connected, valid indices are 0 and 1
        return GamepadIndex < 0 || GamepadIndex >= Gamepad.all.Count;
    }

    // Reads the left analog stick movement and applies deadzone filtering
    private Vector2 GetStickMovement(Gamepad gamepad)
    {
        // Read the current position of the left analog stick
        // Returns a Vector2 where x is horizontal (-1 to 1) and y is vertical (-1 to 1)
        Vector2 stickInput = gamepad.leftStick.ReadValue();
        
        // Check if the stick has moved beyond the deadzone threshold
        if (IsStickInputAboveDeadzone(stickInput))
        {
            // If yes, return the stick input
            return stickInput;
        }
        
        // If stick movement is within the deadzone, return no movement
        // This prevents stick drift (when controllers slowly move on their own)
        return Vector2.zero;
    }

    // Checks if the analog stick input is above the deadzone threshold
    private bool IsStickInputAboveDeadzone(Vector2 stickInput)
    {
        // magnitude is the length of the vector (how far the stick is from center)
        // If it's >= Deadzone (0.2), the stick has moved enough to count
        return stickInput.magnitude >= Deadzone;
    }

    // Checks if the D-pad is being used, and if so, returns D-pad input instead of stick input
    private Vector2 GetDpadMovementIfActive(Gamepad gamepad, Vector2 currentMovement)
    {
        // Read the D-pad input
        // D-pad returns a Vector2 where x is left/right (-1, 0, or 1) and y is up/down (-1, 0, or 1)
        Vector2 dpadInput = gamepad.dpad.ReadValue();
        
        // Check if any D-pad direction is pressed
        if (IsDpadActive(dpadInput))
        {
            // If D-pad is active, use D-pad input (it overrides analog stick)
            return dpadInput;
        }
        
        // If D-pad is not active, keep using the current movement (from analog stick)
        return currentMovement;
    }

    // Checks if the D-pad has any input
    private bool IsDpadActive(Vector2 dpadInput)
    {
        // sqrMagnitude is the squared length of the vector (faster than magnitude)
        // If it's > 0, at least one D-pad direction is pressed
        return dpadInput.sqrMagnitude > 0f;
    }

    // Normalizes diagonal movement so the player doesn't move faster diagonally
    private Vector2 AdjustForDiagonalMovement(Vector2 movement)
    {
        // Check if the movement is diagonal (both x and y are non-zero)
        if (IsDiagonalMovement(movement))
        {
            // Normalize makes the vector length = 1.0
            // Without this, diagonal movement would be 1.414x faster (sqrt(1²+1²))
            movement.Normalize();
        }
        
        // Return the adjusted movement
        return movement;
    }

    // Checks if the movement vector represents diagonal movement
    private bool IsDiagonalMovement(Vector2 movement)
    {
        // sqrMagnitude > 1 means the vector length is > 1
        // This happens when both x and y are non-zero (e.g., x=1, y=1 gives sqrMagnitude=2)
        return movement.sqrMagnitude > 1f;
    }
}