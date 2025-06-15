using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class Manager : MonoBehaviour
{
    public List<Character> Characters = new List<Character>();
    List<BaseClass> BaseClasses = new List<BaseClass>();

    int enemyCount = 1;
    public Player player = new Player();
    public CameraClass cameraClass = new CameraClass();
    public BoxClass boxClass = new BoxClass();
    public int playerLayer, boxLayer, enemyLayer, wallLayer;
    public LayerMask playerMask, boxMask, enemyMask, wallMask;
    public bool hitStop = false;
    float hitStopDuration = 15, hitStopTimer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        boxLayer = LayerMask.NameToLayer("Box");
        wallLayer = LayerMask.NameToLayer("Wall");

        playerMask = LayerMask.GetMask("Player");
        enemyMask = LayerMask.GetMask("Enemy");
        boxMask = LayerMask.GetMask("Box");
        wallMask = LayerMask.GetMask("Wall");

        Characters.Add(player);

        for(int i = 0; i < enemyCount; i++)
        {
            Enemy enemy = new Enemy();
            Characters.Add(enemy);
        }

        foreach (Character character in Characters)
            BaseClasses.Add(character);

        BaseClasses.Add(cameraClass);
        BaseClasses.Add(boxClass);

        foreach (BaseClass _baseClass in BaseClasses)
            _baseClass.Start(this);
    }

    void FixedUpdate()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        if (hitStop == false)
            UpdateEverything();
        else
        {
            if (hitStopTimer != hitStopDuration)
                hitStopTimer++;
            else
            {
                hitStop = false;
                hitStopTimer = 0;
            }
        }
    }

    void UpdateEverything()
    {
        for (int i = Characters.Count - 1; i >= 0; i--)
        {
            Characters[i].Update();
            if (Characters[i].dead)
            {
                Destroy(Characters[i].gameObject);
                Characters.RemoveAt(i);
            }
        }

        cameraClass.Update();
        boxClass.Update();
    }
}

public class BaseClass
{
    public string name = "Character";
    public Manager manager;
    virtual public void Start(Manager _manager) { manager = _manager; }
    virtual public void Update() { }
}

public class CameraClass : BaseClass
{
    GameObject gameObject;
    public bool screenshake;
    float screenShakeTimer, screenshakeDuration = 15, shakeFrequency = 4, shakeMagnitude = 0.1f;
    protected Vector3 initPos;

    public override void Start(Manager _manager)
    {
        gameObject = GameObject.FindGameObjectWithTag("MainCamera");
        base.Start(_manager);
        initPos = gameObject.transform.position;
    }

    public override void Update()
    {
        base.Update();

        if(screenshake)
        {
            float shake = Mathf.Sin(screenShakeTimer * shakeFrequency) * shakeMagnitude;
            gameObject.transform.position = new Vector3(initPos.x + shake, initPos.y + shake,initPos.z + shake);

            screenShakeTimer++;
            if (screenShakeTimer == screenshakeDuration)
            {
                screenshake = false;
                screenShakeTimer = 0;
            }
        }
        else
        {
            gameObject.transform.position = initPos;
        }
    }
}

public class BoxClass : BaseClass
{
    public GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    public BoxCollider2D bc;
    public Rigidbody2D rb;
    protected Vector2 spawnPoint;
    public BoxBehaviour boxBehaviour;
    protected CollisionBehaviour collisionBehaviour = new CollisionBehaviour();
    bool hasHitWall;

    public override void Start(Manager _manager)
    {
        base.Start(_manager);

        gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefab/Box"));

        //Components
        sr = gameObject.GetComponent<SpriteRenderer>();
        boxBehaviour = gameObject.GetComponent<BoxBehaviour>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        boxBehaviour.bc = bc;
        boxBehaviour.rb = rb;
        boxBehaviour.manager = manager;
        boxBehaviour.player = manager.player;

        gameObject.transform.position = spawnPoint;
        gameObject.layer = manager.boxLayer;

        boxBehaviour.ClassStart();
    }

    public override void Update()
    {
        boxBehaviour.ClassUpdate();

        if (collisionBehaviour.CheckCollision(manager.wallMask, bc).hit)
        {
            if (!hasHitWall)
            {
                manager.cameraClass.screenshake = true;
                hasHitWall = true;
            }
        }
        else
            hasHitWall = false;

    }
}