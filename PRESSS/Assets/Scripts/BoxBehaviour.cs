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
    bool isLaunched;
    Vector2 launchDirection;

    bool isGrabbed;

    float defaultDamping;

    LayerMask playerMask;
    LayerMask wallMask;

    CollisionBehaviour collisionBehaviour = new CollisionBehaviour();

    public virtual void ClassStart()
    {
        playerTransform = player.gameObject.transform;
        defaultDamping = rb.linearDamping;

        playerMask = LayerMask.GetMask("Player");
        wallMask = LayerMask.GetMask("Wall");
    }

    public virtual void ClassUpdate()
    {
        DampingCheck();
        GrabCheck();
        LaunchCheck();
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
                Debug.Log("cancel launch");
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
                transform.parent = playerTransform;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.useFullKinematicContacts = true;
                rb.linearDamping = 0;
                rb.linearVelocity = player.gameObject.GetComponent<Rigidbody2D>().linearVelocity;
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
        transform.parent = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void Launch(Vector2 dir)
    {
        isLaunched = true;
        launchDirection = dir;
        rb.linearVelocity = dir * launchSpeed;
    }

    public void CancelLaunch()
    {
        isLaunched = false;
        rb.linearVelocity = Vector2.zero;
    }

    Vector2 LaunchDirection()
    {
        float maxXCollisionDist = bc.size.x / 2 + player.gameObject.GetComponent<BoxCollider2D>().size.x / 2;
        float maxYCollisionDist = bc.size.y / 2 + player.gameObject.GetComponent<BoxCollider2D>().size.y / 2;

        if (Mathf.Abs(playerTransform.position.x - transform.position.x) < maxXCollisionDist)
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

        if (Mathf.Abs(playerTransform.position.y - transform.position.y) < maxYCollisionDist)
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

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == playerTransform)
        {
            if (player.movingEntityBehaviour.isDashing)
            {
                player.movingEntityBehaviour.CancelDash();
                Launch(LaunchDirection());
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            CancelLaunch();
        }
    }*/
}
