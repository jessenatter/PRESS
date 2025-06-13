using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manager : MonoBehaviour
{
    [HideInInspector] public Vector2 moveInput;
    List<Character> Characters = new List<Character>();
    List<BaseClass> BaseClasses = new List<BaseClass>();

    int enemyCount = 1;
    public Player player = new Player();
    CameraClass cameraClass = new CameraClass();

    void Awake()
    {
        Characters.Add(player);

        for(int i = 0; i < enemyCount; i++)
        {
            Enemy enemy = new Enemy();
            Characters.Add(enemy);
        }

        foreach (Character character in Characters)
            BaseClasses.Add(character);

        BaseClasses.Add(cameraClass);

        foreach (BaseClass _baseClass in BaseClasses)
            _baseClass.Start(this);
    }

    void Update()
    {
        foreach (Character _character in Characters)
            _character.Update();

        cameraClass.Update();
    }
}

public class BaseClass
{
    public string name = "Character";
    public Manager manager;
    virtual public void Start(Manager _manager) { manager = _manager; }
    virtual public void Update() { }
}

public class Character : BaseClass
{
    public GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    protected BoxCollider2D bc;
    protected Rigidbody2D rb;
    protected float moveSpeed = 1f, health = 100;
    public MovingEntityBehaviour movingEntityBehaviour = new MovingEntityBehaviour(); //all the posible mechanics of a moving entity
    public InputManager inputManager = new InputManager();
    protected Vector2 moveVec,spawnPoint;

    override public void Start(Manager _manager)
    {
        base.Start(_manager);

        gameObject = new GameObject(name);
        sr = gameObject.AddComponent<SpriteRenderer>();
        bc = gameObject.AddComponent<BoxCollider2D>();
        rb = gameObject.AddComponent<Rigidbody2D>();
        
        sr.sprite = sprite;
        rb.gravityScale = 0;

        movingEntityBehaviour.rb = rb;
        movingEntityBehaviour.moveSpeed = moveSpeed;

        gameObject.transform.position = spawnPoint;
    }

    override public void Update()
    {
        movingEntityBehaviour.Update();
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
    public override void Start(Manager _manager)
    {
        name = "Player";
        sprite = Resources.Load<Sprite>("Sprites/Player");
        moveSpeed = 1.2f;

        inputManager.movingEntityBehaviour = movingEntityBehaviour;
        inputManager.Start();

        base.Start(_manager);
    }

    public override void Update()
    {
        inputManager.Update();
        base.Update();
    }
}

public class Enemy : Character
{
    public override void Start(Manager _manager)
    {
        name = "Enemy";
        sprite = Resources.Load<Sprite>("Sprites/Enemy");
        moveSpeed = 0.7f;
        spawnPoint = new Vector2(2, 2);
        base.Start(_manager);
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

public class CameraClass : BaseClass
{
    float lerpSpeed = 0.5f,zPos = -13;
    GameObject gameObject,target;

    public override void Start(Manager _manager)
    {
        gameObject = GameObject.FindGameObjectWithTag("MainCamera");
        base.Start(_manager);
        target = manager.player.gameObject;
    }

    public override void Update()
    {
        base.Update();

        float _x = Mathf.Lerp(gameObject.transform.position.x, target.transform.position.x, lerpSpeed);
        float _y = Mathf.Lerp(gameObject.transform.position.y, target.transform.position.y, lerpSpeed);

        gameObject.transform.position = new Vector3(_x, _y,zPos);
    }
}