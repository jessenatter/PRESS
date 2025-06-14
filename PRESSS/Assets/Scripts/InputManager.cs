using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    public MovingEntityBehaviour movingEntityBehaviour;

    [Header("Input Actions")]
    InputAction moveAction;
    InputAction dashAction;

    public virtual void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        dashAction = InputSystem.actions.FindAction("Dash");
    }

    public virtual void Update()
    {
        movingEntityBehaviour.moveInput = moveAction.ReadValue<Vector2>();
        movingEntityBehaviour.dashInput = dashAction.WasPressedThisFrame();
    }
}
