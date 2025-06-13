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

    CollisionBehaviour collisionBehaviour = new CollisionBehaviour();

    public virtual void ClassStart()
    {
        playerTransform = player.gameObject.transform;
        defaultDamping = rb.linearDamping;
    }

    public virtual void ClassUpdate()
    {
        //LaunchCheck();
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
        if (Mathf.Abs(playerTransform.position.x - transform.position.x) < bc.size.x / 2)
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

        if (Mathf.Abs(playerTransform.position.y - transform.position.y) < bc.size.y / 2)
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

    private void OnCollisionEnter2D(Collision2D collision)
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
    }
}
