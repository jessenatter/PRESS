using UnityEngine;

public class MovingEntityBehaviour
{
    public Rigidbody2D rb;

    [Header ("Move Settings")]
    public Vector2 moveInput;
    public float moveSpeed = 7; //current move speed

    [Header ("Dash Settings")]
    public bool dashInput;
    public bool dashAble = true;
    public bool isDashing = false;
    public float dashSpeed = 20;
    public float dashMaxTime = 100;
    public float dashTime = 0; //current amount of time dash has been going on
    public float dashMaxCD = 120;
    public float dashCD = 0; //current amount of time dash has been on cooldown

    public Vector2 lastDir;

    public virtual void Update() //called in character class
    {
        DirectionHandler();
        if (!isDashing)
        {
            Move(moveInput);
        }
        Dash(dashInput);
        
    }

    void DirectionHandler()
    {
        if (moveInput != Vector2.zero)
        {
            lastDir = moveInput;
        }
    }

    void Move(Vector2 moveInput)
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    void Dash(bool dashInput)
    {
        if (dashCD > 0)
        {
            dashCD--;
        }
        else
        {
            dashAble = true;
        }

        if (dashTime > 0)
        {
            dashTime--;
        }
        else if (isDashing)
        {
            isDashing = false;
            dashAble = false;
            dashCD = dashMaxCD;
        }

        if (dashInput && dashAble && !isDashing)
        {
            isDashing = true;
            dashTime = dashMaxTime;
            rb.linearVelocity = lastDir.normalized * dashSpeed;
        }
    }
}
