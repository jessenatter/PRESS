using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{
    [HideInInspector] public Manager manager;
    [HideInInspector] public Player player;
    Transform playerTransform;

    [HideInInspector] public BoxCollider2D bc;
    [HideInInspector] public Rigidbody2D rb;

    [Header("Launch Settings")]
    public float launchSpeed;
    public bool isLaunched;
    Vector2 launchDirection;

    LayerMask playerMask;
    LayerMask wallMask;

    bool isGrabbed;

    BoxCollider2D tempBC;
    float defaultDamping;

    CollisionBehaviour collisionBehaviour = new CollisionBehaviour();

    public virtual void ClassStart()
    {
        playerTransform = player.gameObject.transform;
        defaultDamping = rb.linearDamping;

        tempBC = player.gameObject.AddComponent<BoxCollider2D>(); //stop the box from going through the wall

        playerMask = LayerMask.GetMask("Player");
        wallMask = LayerMask.GetMask("Wall");
    }

    public virtual void ClassUpdate()
    {
        GrabCheck();
        LaunchCheck();
        DampingCheck();
        WallCheck();
    }

    void DampingCheck()
    {
        if (isGrabbed || isLaunched)
        {
            rb.linearDamping = 0;
        }
        else
        {
            rb.linearDamping = defaultDamping;
        }
    }

    void LaunchCheck()
    {
        if (collisionBehaviour.CheckCollision(playerMask, bc).hit)
        {
            if (player.movingEntityBehaviour.isDashing)
            {
                player.movingEntityBehaviour.CancelDash();
                CancelGrab();
                Launch(LaunchDirection());
            }
        }
    }

    void WallCheck()
    {
        RaycastHit2D directionRay = Physics2D.Raycast(transform.position, launchDirection, 0.6f, wallMask);

        if (directionRay.collider != null)
        {
            if (isLaunched)
            {
                manager.cameraClass.screenshake = true;
                CancelLaunch();
            }
        }
    }

    void GrabCheck()
    {
        if (collisionBehaviour.CheckCollision(playerMask, bc).hit)
        {
            if (player.movingEntityBehaviour.isGrabbing)
            {
                isGrabbed = true;

                tempBC.size = bc.size;
                tempBC.offset = new Vector2(transform.position.x - playerTransform.position.x, transform.position.y - playerTransform.position.y);

                tempBC.enabled = true;

                transform.parent = playerTransform;

                rb.simulated = false;
            }
        }

        if (!player.movingEntityBehaviour.isGrabbing)
        {
            if (isGrabbed)
            {
                CancelGrab();
            }
        }
    }

    public void CancelGrab()
    {
        isGrabbed = false;

        rb.simulated = true;
        transform.parent = null;

        bc.enabled = true;
        tempBC.enabled = false;
    }

    void Launch(Vector2 dir)
    {
        isLaunched = true;
        launchDirection = dir;
        rb.linearVelocity = dir * launchSpeed;
        rb.linearDamping = 0;
    }

    public void CancelLaunch()
    {
        isLaunched = false;
        rb.linearVelocity = Vector2.zero;
        rb.linearDamping = defaultDamping;
    }

    Vector2 LaunchDirection()
    {
        if (Mathf.Abs(playerTransform.position.x - transform.position.x) < (bc.size.x / 2 + player.gameObject.GetComponent<BoxCollider2D>().size.x / 2))
        {
            if (playerTransform.position.y > transform.position.y)
            {
                return Vector2.down;
            }
            else
            {
                return Vector2.up;
            }
        }

        if (Mathf.Abs(playerTransform.position.y - transform.position.y) < (bc.size.y / 2 + player.gameObject.GetComponent<BoxCollider2D>().size.y / 2))
        {
            if (playerTransform.position.x > transform.position.x)
            {
                return Vector2.left;
            }
            else
            {
                return Vector2.right;
            }
        }

        return Vector2.zero;
    }
}
