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
        LaunchCheck();
        WallCheck();
    }

    void LaunchCheck()
    {
        if (collisionBehaviour.CheckCollision(playerMask, bc).hit)
        {
            if (player.movingEntityBehaviour.isDashing)
            {
                player.movingEntityBehaviour.CancelDash();
                Launch(LaunchDirection());
            }
        }
    }

    void WallCheck()
    {
        RaycastHit2D directionRay = Physics2D.Raycast(transform.position, rb.linearVelocity, 0.6f, wallMask);

        if (directionRay.collider != null)
        {
            CancelLaunch();
        }
    }

    void Launch(Vector2 dir)
    {
        rb.linearDamping = 0;
        rb.linearVelocity = dir * launchSpeed;
    }

    public void CancelLaunch()
    {
        rb.linearDamping = defaultDamping;
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
