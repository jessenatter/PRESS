using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Character : BaseClass
{
    public GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    public BoxCollider2D bc;
    protected Rigidbody2D rb;
    protected float health = 100;
    public MovingEntityBehaviour movingEntityBehaviour; //all the posible mechanics of a moving entity
    protected Vector2 moveVec, spawnPoint;
    public CollisionBehaviour collisionBehaviour;
    public bool dead;

    override public void Start(Manager _manager)
    {
        base.Start(_manager);

        gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefab/Character"));
        gameObject.name = name;

        //Components
        sr = gameObject.GetComponent<SpriteRenderer>();
        movingEntityBehaviour = gameObject.GetComponent<MovingEntityBehaviour>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        collisionBehaviour = new CollisionBehaviour();

        sr.sprite = sprite;
        movingEntityBehaviour.rb = rb;

        gameObject.transform.position = spawnPoint;
    }

    override public void Update()
    {
        base.Update();
        movingEntityBehaviour.ClassUpdate();
    }

    public virtual void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        dead = true;
    }
}

public class Player : Character
{
    public InputManager inputManager = new InputManager();
    public override void Start(Manager _manager)
    {
        name = "Player";
        sprite = Resources.Load<Sprite>("Sprites/Player");

        base.Start(_manager);

        gameObject.layer = manager.playerLayer;

        if (movingEntityBehaviour != null)
        {
            inputManager.movingEntityBehaviour = movingEntityBehaviour;
            inputManager.Start();
        }
    }

    public override void Update()
    {
        base.Update();

        if (movingEntityBehaviour != null)
        {
            inputManager.Update();
        }

        if (collisionBehaviour.CheckCollision(manager.enemyMask, bc).hit)
        {
            
        }
    }
}

public class Enemy : Character
{
    bool attachedToBox;
    //protected float attachToBoxMinSpeed = 0.2f, attachToBoxMinForce = 0.2f;
    float squishSpeed = 0.001f;

    public override void Start(Manager _manager)
    {
        name = "Enemy";
        sprite = Resources.Load<Sprite>("Sprites/Enemy");
        spawnPoint = new Vector2(2, 2);

        base.Start(_manager);

        gameObject.layer = manager.enemyLayer;

        movingEntityBehaviour.moveSpeed = 2.5f;
    }

    public override void Update()
    {
        Vector2 playerEnemyVector = manager.player.gameObject.transform.position - gameObject.transform.position;

        if (Mathf.Round(playerEnemyVector.magnitude) == 0 || attachedToBox)
            movingEntityBehaviour.moveInput = Vector2.zero;
        else
            movingEntityBehaviour.moveInput = playerEnemyVector;

        base.Update();

        AttachBoxCheck();

        if (collisionBehaviour.CheckCollision(manager.boxMask, bc).hit)
        {
            /*if (manager.boxClass.rb.linearVelocity.magnitude > attachToBoxMinSpeed)
            {
                if (!attachedToBox)
                    SetAttachedToBox(true);
            }
            else if(attachedToBox)
                SetAttachedToBox(false);

            if(manager.boxClass.rb.totalForce.magnitude > attachToBoxMinForce)
            {
                if (!attachedToBox)
                    SetAttachedToBox(true);
            }*/

            

            if (collisionBehaviour.CheckCollision(manager.wallMask, bc).hit)
            {
                if(manager.boxClass.boxBehaviour.isLaunched)
                    Die();
                else if(attachedToBox)
                {
                    float scaleX = Mathf.Clamp(gameObject.transform.localScale.x - SquishDirection().x * squishSpeed, 0, Mathf.Infinity);
                    float scaleY = Mathf.Clamp(gameObject.transform.localScale.y - SquishDirection().y * squishSpeed, 0, Mathf.Infinity);
                    gameObject.transform.localScale = new Vector2(scaleX, scaleY);
                }
            }
        }

        if (gameObject.transform.localScale.magnitude <= 1.05f)
        {
            Die();
        }
    }

    void AttachBoxCheck()
    {
        RaycastHit2D checkRay = Physics2D.BoxCast(
            origin : manager.boxClass.gameObject.transform.position, 
            size : manager.boxClass.bc.size,
            angle : 0,
            direction : manager.player.movingEntityBehaviour.moveInput, 
            distance : 0.1f, 
            layerMask : manager.enemyMask);

        CollisionBehaviour.Collision playerBoxCollision = collisionBehaviour.CheckCollision(manager.boxMask, manager.player.bc);
        CollisionBehaviour.Collision enemyBoxCollision = collisionBehaviour.CheckCollision(manager.boxMask, bc);

        if ((checkRay.collider == bc && playerBoxCollision.hit) || (manager.boxClass.boxBehaviour.isLaunched && enemyBoxCollision.hit))
        {
            SetAttachedToBox(true);
        }
        else if (attachedToBox)
        {
            SetAttachedToBox(false);
        }
    }

    void SetAttachedToBox(bool attached)
    {
        if(attached)
        {
            gameObject.transform.SetParent(manager.boxClass.gameObject.transform);
            attachedToBox = true;
        }
        else
        {
            gameObject.transform.SetParent(manager.player.gameObject.transform.parent);
            attachedToBox = false;
        }
    }

    Vector3 SquishDirection()
    {
        UnityEngine.Transform boxTransform = manager.boxClass.gameObject.transform;

        Vector2 boxEnemyDistance = new Vector2(Mathf.Abs(boxTransform.position.x - gameObject.transform.position.x), Mathf.Abs(boxTransform.position.y - gameObject.transform.position.y));
        if (boxEnemyDistance.x <= boxEnemyDistance.y)
        {
            return Vector2.up;
        }
        else
        {
            return Vector2.right;
        }

        /*RaycastHit2D downRay = Physics2D.BoxCast(gameObject.transform.position, bc.size * 0.9f, 0, Vector2.down, 0.1f, manager.boxMask);
        RaycastHit2D upRay = Physics2D.BoxCast(gameObject.transform.position, bc.size * 0.9f, 0, Vector2.up, 0.1f, manager.boxMask);
        RaycastHit2D leftRay = Physics2D.BoxCast(gameObject.transform.position, bc.size * 0.9f, 0, Vector2.left, 0.1f, manager.boxMask);
        RaycastHit2D rightRay = Physics2D.BoxCast(gameObject.transform.position, bc.size * 0.9f, 0, Vector2.right, 0.1f, manager.boxMask);

        if (downRay.collider != null || upRay.collider != null)
        {
            return Vector2.up;
        }

        if (leftRay.collider != null || rightRay.collider != null)
        {
            return Vector2.right;
        }*/

        /*if (Mathf.Abs(boxTransform.position.x - gameObject.transform.position.x) < (bc.size.x / 2 + boxTransform.gameObject.GetComponent<BoxCollider2D>().size.x / 2))
        {
            return Vector2.right;
        }

        if (Mathf.Abs(boxTransform.position.y - gameObject.transform.position.y) < (bc.size.y / 2 + boxTransform.gameObject.GetComponent<BoxCollider2D>().size.y / 2))
        {
            return Vector2.up;
        }*/

        return Vector2.zero;
    }

}
