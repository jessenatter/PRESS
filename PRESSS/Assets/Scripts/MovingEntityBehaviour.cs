using UnityEngine;

public class MovingEntityBehaviour : MonoBehaviour
{
    [HideInInspector] public Manager manager;
    [HideInInspector] public Enemy enemy; // only applicable if the character is an enemy

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D bc;

    [Header ("Move Settings")]
    [HideInInspector] public Vector2 moveInput;
    public float moveSpeed = 7; //current move speed

    [Header ("Dash Settings")]
    [HideInInspector] public bool dashInput;
    [HideInInspector] public bool dashAble = true;
    [HideInInspector] public bool isDashing = false;
    public float dashSpeed = 30;
    public float dashMaxTime = 20;
    [HideInInspector] public float dashTime = 0; //current amount of time dash has been going on
    public float dashMaxCD = 120;
    [HideInInspector] public float dashCD = 0; //current amount of time dash has been on cooldown

    [HideInInspector] public bool grabInput;
    [HideInInspector] public bool isGrabbing;

    [HideInInspector] public Vector2 lastDir;

    [HideInInspector] public CollisionBehaviour cb = new CollisionBehaviour();

    public virtual void ClassUpdate() //called in character base class
    {
        DirectionHandler();
        if (!isDashing)
        {
            Move(moveInput);
        }
        Dash(dashInput);
        Grab(grabInput);

        WallCheck();
        
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
        //Debug.Log("DASH INPUT: " + dashInput.ToString() + "DASH ABLE: " + dashAble.ToString() + "IS DASHING: " + isDashing.ToString());

        if (dashCD > 0)
        {
            dashCD--;
        }
        else if (!dashAble)
        {
            dashAble = true;
        }

        if (dashTime > 0)
        {
            dashTime--;
        }
        else if (isDashing)
        {
            CancelDash();
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

        rb.linearVelocity = Vector2.zero;

        dashTime = 0;
        dashCD = dashMaxCD;
    }

    void Grab (bool grabInput)
    {
        isGrabbing = grabInput;
    }

    void WallCheck()
    {
        if (cb.CheckCollision(manager.wallMask, bc).hit)
        {
            RaycastHit2D wallDashRay = Physics2D.Raycast(transform.position, lastDir, 1, manager.wallMask);
            if (isDashing && wallDashRay.collider != null) //checks to see if the object is dashing into the wall
            {
                CancelDash();
            }
        }
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
