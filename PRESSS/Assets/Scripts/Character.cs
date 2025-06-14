using UnityEngine;

public class Character : BaseClass
{
    public GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    public BoxCollider2D bc;
    protected Rigidbody2D rb;
    protected float moveSpeed = 1f, deceleration = 0.05f, health = 100, maxSpeed = 3f;
    public MovingEntityBehaviour movingEntityBehaviour; //all the posible mechanics of a moving entity
    protected Vector2 moveVec, spawnPoint;
    public CollisionBehaviour collisionBehaviour;

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

    }
}

public class Player : Character
{
    protected LayerMask enemyMask;
    public InputManager inputManager = new InputManager();
    public override void Start(Manager _manager)
    {
        name = "Player";
        sprite = Resources.Load<Sprite>("Sprites/Player");

        base.Start(_manager);

        enemyMask = manager.enemyMask;
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

        //if (collisionBehaviour.CheckCollision(enemyMask, bc))
        //{
            
        //}
    }
}

public class Enemy : Character
{
    protected LayerMask boxMask;
    bool attachedToBox;
    float boxAttachSpeed = 0.2f,boxAttachCD = 15,boxAttachTimer;

    public override void Start(Manager _manager)
    {
        name = "Enemy";
        sprite = Resources.Load<Sprite>("Sprites/Enemy");
        moveSpeed = 0.8f;
        spawnPoint = new Vector2(2, 2);

        base.Start(_manager);

        boxMask = manager.boxMask;
        gameObject.layer = manager.enemyLayer;

        movingEntityBehaviour.moveSpeed = 3.5f;
        boxAttachTimer = boxAttachCD;
    }

    public override void Update()
    {
        Vector2 playerEnemyVector = manager.player.gameObject.transform.position - gameObject.transform.position;

        if (Mathf.Round(playerEnemyVector.magnitude) == 0 || attachedToBox)
            movingEntityBehaviour.moveInput = Vector2.zero;
        else
            movingEntityBehaviour.moveInput = playerEnemyVector;

        base.Update();

        if (collisionBehaviour.CheckCollision(boxMask, bc) && collisionBehaviour.CheckCollision(boxMask, manager.player.bc))
        {
            if (manager.boxClass.rb.linearVelocity.magnitude > boxAttachSpeed)
            {
                if (!attachedToBox && boxAttachTimer == boxAttachCD)
                    AttachedToBox(true);
            }
            else if (attachedToBox)
                AttachedToBox(false);
        }
        else if (attachedToBox)
            AttachedToBox(false);

        if(!attachedToBox && boxAttachTimer != boxAttachCD)
            boxAttachTimer++;
    }

    void AttachedToBox(bool attached)
    {
        if(attached)
        {
            gameObject.transform.SetParent(manager.boxClass.gameObject.transform);
            attachedToBox = true;
            boxAttachTimer = 0;
        }
        else
        {
            gameObject.transform.SetParent(manager.player.gameObject.transform.parent);
            attachedToBox = false;
        }
    }
}
