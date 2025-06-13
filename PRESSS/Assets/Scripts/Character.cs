using UnityEngine;

public class Character : BaseClass
{
    public GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    protected BoxCollider2D bc;
    protected Rigidbody2D rb;
    protected float moveSpeed = 1f, deceleration = 0.05f, health = 100, maxSpeed = 3f;
    public MovingEntityBehaviour movingEntityBehaviour; //all the posible mechanics of a moving entity
    protected Vector2 moveVec, spawnPoint;

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

        sr.sprite = sprite;
        movingEntityBehaviour.rb = rb;

        gameObject.transform.position = spawnPoint;
    }

    override public void Update()
    {
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
    public InputManager inputManager = new InputManager();
    public override void Start(Manager _manager)
    {
        name = "Player";
        sprite = Resources.Load<Sprite>("Sprites/Player");
        //moveSpeed = 2f;
        //maxSpeed = 5;

        base.Start(_manager);

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
    }
}

public class Enemy : Character
{
    public override void Start(Manager _manager)
    {
        name = "Enemy";
        sprite = Resources.Load<Sprite>("Sprites/Enemy");
        moveSpeed = 1.2f;
        spawnPoint = new Vector2(2, 2);

        base.Start(_manager);

        movingEntityBehaviour.moveSpeed = 3.5f;
    }

    public override void Update()
    {
        Vector2 playerEnemyVector = manager.player.gameObject.transform.position - gameObject.transform.position;

        if (Mathf.Round(playerEnemyVector.magnitude) == 0)
            movingEntityBehaviour.moveInput = Vector2.zero;
        else
            movingEntityBehaviour.moveInput = playerEnemyVector;

        base.Update();
    }
}
