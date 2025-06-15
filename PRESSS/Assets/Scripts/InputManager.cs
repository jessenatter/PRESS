using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    public MovingEntityBehaviour movingEntityBehaviour;

    [Header("Input Actions")]
    InputAction moveAction;
    InputAction dashAction;
    InputAction grabAction;

    bool dashInputRecieved;

    public virtual void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        dashAction = InputSystem.actions.FindAction("Dash");
        grabAction = InputSystem.actions.FindAction("Grab");
    }

    public virtual void Update()
    {
        Debug.Log(dashAction.WasPressedThisFrame());

        movingEntityBehaviour.moveInput = moveAction.ReadValue<Vector2>();

        if (movingEntityBehaviour.dashInput) // if dash input is true set it to false
        {
            movingEntityBehaviour.dashInput = false;
        }

        if (dashAction.IsPressed() && !dashInputRecieved) // if the dash action is pressed and the dash input has not been recieved already, set the dash input to true
        {
            dashInputRecieved = true;
            movingEntityBehaviour.dashInput = true;
        }

        if (!dashAction.IsPressed() && dashInputRecieved) // when dash action is not pressed set the dash input recieved to false
        {
            dashInputRecieved = false;
        }

        //movingEntityBehaviour.dashInput = dashAction.IsPressed();
        movingEntityBehaviour.grabInput = grabAction.IsPressed();
    }

}
