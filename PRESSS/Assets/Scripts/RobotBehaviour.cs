using UnityEngine;

public class RobotBehaviour : MonoBehaviour
{
    [HideInInspector] public Manager manager;
    [HideInInspector] public Player player;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D bc;
    [HideInInspector] public SpriteRenderer sr;

    CollisionBehaviour cb = new();

    public int maxChargeAmount = 1200;
    public int chargeAmount = 600;

    bool isGrabbed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ClassStart()
    {
        
    }

    // Update is called once per frame
    public void ClassUpdate()
    {
        Debug.Log("CHARGE AMOUNT: " + chargeAmount.ToString());
        TakeDamage();
        GrabCheck();
    }

    public void Charge(int addedCharge)
    {
        if (chargeAmount + addedCharge < maxChargeAmount)
        {
            chargeAmount += addedCharge;
        }
        else
        {
            chargeAmount = maxChargeAmount;
        }
        
    }

    void GrabCheck()
    {
        if (cb.CheckCollision(manager.playerMask, bc).hit)
        {
            if (player.movingEntityBehaviour.isGrabbing)
            {
                isGrabbed = true;
            }
        }

        if (isGrabbed)
        {
            rb.linearVelocity = player.movingEntityBehaviour.rb.linearVelocity;
        }

        if (!player.movingEntityBehaviour.isGrabbing || !cb.CheckCollision(manager.playerMask, bc).hit)
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
    }

    void TakeDamage()
    {
        CollisionBehaviour.Collision collision = cb.CheckCollision(manager.enemyMask, bc);

        if (collision.hit)
        {
            if (chargeAmount > collision.collider.gameObject.GetComponent<MovingEntityBehaviour>().enemy.damage) // get the enemy's damage
            {
                chargeAmount -= collision.collider.gameObject.GetComponent<MovingEntityBehaviour>().enemy.damage;
                collision.collider.gameObject.GetComponent<MovingEntityBehaviour>().enemy.Die();
            }
            else
            {
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("DIE");
    }
}
