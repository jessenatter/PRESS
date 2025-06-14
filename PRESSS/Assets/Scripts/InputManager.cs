using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    public MovingEntityBehaviour movingEntityBehaviour;

    [Header("Input Actions")]
    InputAction moveAction;
    InputAction dashAction;
    InputAction grabAction;

    public virtual void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        dashAction = InputSystem.actions.FindAction("Dash");
        grabAction = InputSystem.actions.FindAction("Grab");
    }

    public virtual void Update()
    {
        movingEntityBehaviour.moveInput = moveAction.ReadValue<Vector2>();
        movingEntityBehaviour.dashInput = dashAction.WasPressedThisFrame();
        movingEntityBehaviour.grabInput = grabAction.IsPressed();
    }

}
