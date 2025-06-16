using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using TMPro;

public class Manager : MonoBehaviour
{
    public List<Character> Characters = new List<Character>();
    List<BaseClass> BaseClasses = new List<BaseClass>();

    public Player player = new Player();
    public CameraClass cameraClass = new CameraClass();
    public BoxClass boxClass = new BoxClass();
    public WaveManager waveManager = new WaveManager();
    [HideInInspector] public int playerLayer, boxLayer, enemyLayer, wallLayer, score;
    [HideInInspector] public LayerMask playerMask, boxMask, enemyMask, wallMask;

    public bool hitStop = false;
    float hitStopTimer;

    public TextMeshProUGUI scoreUI, waveUI;

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

        foreach (Character character in Characters)
            BaseClasses.Add(character);

        BaseClasses.Add(cameraClass);
        BaseClasses.Add(boxClass);
        BaseClasses.Add(waveManager);

        foreach (BaseClass _baseClass in BaseClasses)
            _baseClass.Start(this);
    }

    void FixedUpdate()
    {
        if (Keyboard.current.rKey.IsPressed()) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        Gameplay();
    }

    void Gameplay()
    {
        if (hitStop == false)
            UpdateEverything();
        else
        {
            if (hitStopTimer > 0)
                hitStopTimer--;
            else
                hitStop = false;
        }

        scoreUI.text = "SCORE: " + score.ToString();
    }

    void UpdateEverything()
    {
        for (int i = Characters.Count - 1; i >= 0; i--)
        {
            Characters[i].Update();
            if (Characters[i].dead)
            {
                if (Characters[i] is Enemy)
                    waveManager.currentEnemyCount--;

                Destroy(Characters[i].gameObject);
                Characters.RemoveAt(i);
            }
        }

        cameraClass.Update();
        boxClass.Update();
        waveManager.Update();
    }

    public void StartHitstop(float duration)
    {
        hitStopTimer = duration;
        hitStop = true;
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
    float screenShakeTimer, screenshakeDuration = 20, shakeFrequency = 1, shakeMagnitude = 0.1f;
    public Vector2 screenShakeDir; // more like axis
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

        IdleMovement();

        if(screenshake)
        {
            float shake = Mathf.Sin(screenShakeTimer * shakeFrequency) * shakeMagnitude;
            gameObject.transform.position = new Vector3(initPos.x + shake * screenShakeDir.x, initPos.y + shake * screenShakeDir.y, initPos.z);

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

    public void Screenshake(Vector2 direction, float magnitude, float duration, float frequency)
    {
        screenshake = true;
        screenShakeDir = direction;
        shakeMagnitude = magnitude;
        screenshakeDuration = duration;
        shakeFrequency = frequency;
    }

    void IdleMovement()
    {
        float xRotate = Mathf.Sin(Time.time) * 0.5f;
        float yRotate = Mathf.Cos(Time.time);
        gameObject.transform.Rotate(new Vector3(xRotate * 0.005f, yRotate * 0.005f, 0));
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
    //bool hasHitWall;

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

        //there is already a wall check this is redundant

        /*if (collisionBehaviour.CheckCollision(manager.wallMask, bc).hit)
        {
            if (!hasHitWall)
            {
                manager.cameraClass.screenshake = true;
                hasHitWall = true;
            }
        }
        else
            hasHitWall = false;*/

    }
}