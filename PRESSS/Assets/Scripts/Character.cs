using UnityEngine;

public class Character : BaseClass
{
    public GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    protected BoxCollider2D bc;
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
    protected float attachToBoxMinSpeed = 0.2f, attachToBoxMinForce = 0.2f;

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

        if (collisionBehaviour.CheckCollision(manager.boxMask, bc).hit)
        {
            if (manager.boxClass.rb.linearVelocity.magnitude > attachToBoxMinSpeed)
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
            }

            if (collisionBehaviour.CheckCollision(manager.wallMask, bc).hit)
            {
                if(manager.boxClass.boxBehaviour.isLaunched)
                    Die();
            }
        }
        else if(attachedToBox)
            SetAttachedToBox(false);
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

}
