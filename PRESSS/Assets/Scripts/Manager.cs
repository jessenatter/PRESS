using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private InputSystem_Actions input;
    [HideInInspector] public Vector2 moveInput;
    List<character> Characters = new List<character>();
    int enemyCount = 1;
    public player player = new player();

    void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        Characters.Add(player);

        for(int i = 0; i < enemyCount; i++)
        {
            enemy enemy = new enemy();
            Characters.Add(enemy);
        }

        foreach (character _character in Characters)
            _character.Start(this);
    }

    void Update()
    {
        foreach (character _character in Characters)
            _character.Update();
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();
}

public class character
{
    protected string name = "character";
    public GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    protected BoxCollider2D bc;
    protected Rigidbody2D rb;
    protected float moveSpeed = 1f, deceleration = 0.05f, health = 100, maxSpeed = 3f;
    public Manager manager;
    protected Vector2 moveVec,spawnPoint;

    public virtual void Start(Manager _manager)
    {
        manager = _manager;
        gameObject = new GameObject(name);
        sr = gameObject.AddComponent<SpriteRenderer>();
        bc = gameObject.AddComponent<BoxCollider2D>();
        rb = gameObject.AddComponent<Rigidbody2D>();
        sr.sprite = sprite;
        rb.gravityScale = 0;
        gameObject.transform.position = spawnPoint;
    }

    public virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (moveVec.x == 0 && moveVec.y == 0)
        {
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, deceleration);
            rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, 0, deceleration);
        }
        else
        {
            rb.AddForce(moveVec * moveSpeed);
        }

        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
        rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxSpeed, maxSpeed);
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

public class player : character
{
    public override void Start(Manager _manager)
    {
        name = "Player";
        sprite = Resources.Load<Sprite>("Sprites/Player");
        moveSpeed = 2f;
        maxSpeed = 5;
        base.Start(_manager);
    }

    public override void Update()
    {
        moveVec = manager.moveInput;
        base.Update();
    }
}

public class enemy : character
{
    public override void Start(Manager _manager)
    {
        name = "Enemy";
        sprite = Resources.Load<Sprite>("Sprites/Enemy");
        moveSpeed = 1.2f;
        spawnPoint = new Vector2(2, 2);
        base.Start(_manager);
    }

    public override void Update()
    {
        Vector2 playerEnemyVector = manager.player.gameObject.transform.position - gameObject.transform.position;

        if (Mathf.Round(playerEnemyVector.magnitude) == 0)
            moveVec = Vector2.zero;
        else
            moveVec = playerEnemyVector;

        base.Update();
    }
}