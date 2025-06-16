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
    public Vector2 moveVec, spawnPoint = new Vector2(-2, 0);
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
        movingEntityBehaviour.bc = bc;
        movingEntityBehaviour.manager = manager;

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

    public virtual void Die()
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
        sprite = Resources.Load<Sprite>("Sprites/Player/ghost_girl_front");

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
    
    float squishSpeed = 0.01f;
    int scoreValue = 100;

    public int damage = 100;
    public GameObject target;

    public override void Start(Manager _manager)
    {
        name = "Enemy";
        sprite = Resources.Load<Sprite>("Sprites/ghost");

        base.Start(_manager);

        gameObject.layer = manager.enemyLayer;
        target = manager.robotClass.gameObject;

        movingEntityBehaviour.moveSpeed = 1f;
        movingEntityBehaviour.enemy = this;

        sr.material = Resources.Load<Material>("Materials/Ghost");
    }

    public override void Update()
    {
        Vector2 moveVector = target.transform.position - gameObject.transform.position;

        if (Mathf.Round(moveVector.magnitude) == 0 || attachedToBox)
            movingEntityBehaviour.moveInput = Vector2.zero;
        else
            movingEntityBehaviour.moveInput = moveVector;

        base.Update();

        FlipSprite();
        AttachBoxCheck();

        if (collisionBehaviour.CheckCollision(manager.boxMask, bc).hit)
        {
            if (collisionBehaviour.CheckCollision(manager.wallMask, bc).hit)
            {
                if (manager.boxClass.boxBehaviour.isLaunched)
                {
                    manager.score += scoreValue;
                    manager.robotClass.robotBehaviour.Charge(scoreValue);
                    Die();
                }
                else if (attachedToBox)
                {
                    float scaleX = Mathf.Clamp(gameObject.transform.localScale.x - SquishDirection().x * squishSpeed, 0, Mathf.Infinity);
                    float scaleY = Mathf.Clamp(gameObject.transform.localScale.y - SquishDirection().y * squishSpeed, 0, Mathf.Infinity);
                    gameObject.transform.localScale = new Vector2(scaleX, scaleY);
                }
            }
        }

        if (gameObject.transform.localScale.magnitude <= 1.05f)
        {
            manager.score += scoreValue * 5;
            manager.robotClass.robotBehaviour.Charge(scoreValue * 2);
            Die();
        }
    }

    void FlipSprite()
    {
        if (movingEntityBehaviour.moveInput.x > 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }

    void AttachBoxCheck()
    {
        RaycastHit2D checkRay = Physics2D.BoxCast( //the ray that checks if the player is moving towards the enemy (the ray comes from the box)
            origin : manager.boxClass.gameObject.transform.position, 
            size : manager.boxClass.bc.size,
            angle : 0,
            direction : manager.player.movingEntityBehaviour.moveInput, 
            distance : 0.1f, 
            layerMask : manager.enemyMask);

        CollisionBehaviour.Collision playerBoxCollision = collisionBehaviour.CheckCollision(manager.boxMask, manager.player.bc); //checks if the player is colliding with the box
        CollisionBehaviour.Collision enemyBoxCollision = collisionBehaviour.CheckCollision(manager.boxMask, bc); // checks if the enemy is colliding with the box

        if ((checkRay.collider == bc && playerBoxCollision.hit) || (manager.boxClass.boxBehaviour.isLaunched && enemyBoxCollision.hit))
        {
            SetAttachedToBox(true);
        }
        else if (attachedToBox)
        {
            SetAttachedToBox(false);
        }

        if (!enemyBoxCollision.hit)
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

            if (manager.boxClass.boxBehaviour.isLaunched)
            {
                rb.simulated = false;
            }
        }
        else
        {
            gameObject.transform.SetParent(manager.player.gameObject.transform.parent);
            rb.simulated = true;
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
    }

    public override void Die()
    {
        base.Die();

        GameObject blood = Object.Instantiate(Resources.Load<GameObject>("Prefab/BloodParticles"));
        blood.transform.position = gameObject.transform.position;
        manager.cameraClass.screenshake = true;
    }
}
