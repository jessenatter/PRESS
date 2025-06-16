using UnityEngine;

public class RobotBehaviour : MonoBehaviour
{
    [HideInInspector] public Manager manager;
    [HideInInspector] public Player player;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D bc;
    [HideInInspector] public SpriteRenderer sr;

    CollisionBehaviour cb = new();

    public float maxChargeAmount = 1200;
    public float chargeAmount = 600;

    bool isGrabbed;

    GameObject charge;
    float chargeYinitScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ClassStart()
    {
        charge = gameObject.transform.Find("Charge").gameObject;
        chargeYinitScale = charge.transform.localScale.y;
    }

    // Update is called once per frame
    public void ClassUpdate()
    {
        float chargeOutOf1 = chargeAmount / maxChargeAmount;
        charge.gameObject.transform.localScale = new Vector2(charge.transform.localScale.x, chargeYinitScale * chargeOutOf1);

        TakeDamage();
        GrabCheck();
    }

    public void Charge(int addedCharge)
    {
        if (chargeAmount + addedCharge < maxChargeAmount)
        {
            chargeAmount += addedCharge;
            //spark
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
            GameObject sparks = Object.Instantiate(Resources.Load<GameObject>("Prefab/RobotParticles"));
            sparks.transform.position = gameObject.transform.position;

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

        for (int i = 0; i < 5; i++)
        {
            GameObject sparks = Object.Instantiate(Resources.Load<GameObject>("Prefab/RobotParticles"));
            sparks.transform.position = gameObject.transform.position;
        }
    }
}
