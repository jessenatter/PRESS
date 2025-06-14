using UnityEngine;

public class MovingEntityBehaviour : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;

    [Header ("Move Settings")]
    [HideInInspector] public Vector2 moveInput;
    public float moveSpeed = 7; //current move speed

    [Header ("Dash Settings")]
    [HideInInspector] public bool dashInput;
    [HideInInspector] public bool dashAble = true;
    [HideInInspector] public bool isDashing = false;
    public float dashSpeed = 20;
    public float dashMaxTime = 100;
    [HideInInspector] public float dashTime = 0; //current amount of time dash has been going on
    public float dashMaxCD = 120;
    [HideInInspector] public float dashCD = 0; //current amount of time dash has been on cooldown

    [HideInInspector] public bool grabInput;
    [HideInInspector] public bool isGrabbing;

    [HideInInspector] public Vector2 lastDir;

    public virtual void ClassUpdate() //called in character class
    {
        DirectionHandler();
        if (!isDashing)
        {
            Move(moveInput);
        }
        Dash(dashInput);
        Grab(grabInput);
        
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

    public void CancelDash()
    {
        isDashing = false;
        dashAble = true;

        dashTime = 0;
        dashCD = dashMaxCD;
    }

    void Grab (bool grabInput)
    {
        isGrabbing = grabInput;
    }
}

public class CollisionBehaviour
{
    public struct Collision
    {
        public bool hit;
        public Collider2D collider;

        public Collision (bool hit, Collider2D collider)
        {
            this.hit = hit;
            this.collider = collider;
        }
    }
    public Collision CheckCollision(LayerMask layer,BoxCollider2D bc)
    {
        bool hit = false;
        Collider2D box = Physics2D.OverlapBox(bc.transform.position, bc.size, 0, layer);

        if (box != null)
        {
            hit = true;
        }

        Collision returnValue = new Collision(hit, box);

        return returnValue;
    }
}
