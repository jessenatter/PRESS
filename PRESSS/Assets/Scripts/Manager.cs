using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using TMPro;
using UnityEngine.Rendering;

public class Manager : MonoBehaviour
{
    public List<Character> Characters = new List<Character>();
    List<BaseClass> BaseClasses = new List<BaseClass>();
    public List<Rigidbody2D> rbs = new List<Rigidbody2D>();
    public Vector2[] storedVelocities;

    public Player player = new Player();
    public CameraClass cameraClass = new CameraClass();
    public BoxClass boxClass = new BoxClass();
    public RobotClass robotClass = new RobotClass();
    public WaveManager waveManager = new WaveManager();
    [HideInInspector] public int playerLayer, boxLayer, enemyLayer, wallLayer, score;
    [HideInInspector] public LayerMask playerMask, boxMask, enemyMask, wallMask;

    public bool hitStop = false;
    float hitStopTimer;

    public TextMeshProUGUI scoreUI, waveUI;
    public GameObject upgradeMenu;

    void Awake()
    {
        Upgrades.upgradeMenu = upgradeMenu;
        Upgrades.manager = this;
        Upgrades.Start();

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
        {
            BaseClasses.Add(character);
        }

        BaseClasses.Add(cameraClass);
        BaseClasses.Add(boxClass);
        BaseClasses.Add(waveManager);
        BaseClasses.Add(robotClass);

        foreach (BaseClass _baseClass in BaseClasses)
            _baseClass.Start(this);

        cameraClass.gameObject.GetComponent<AudioSource>().volume = GameDataManager.musicVolume * 0.5f;
        cameraClass.gameObject.transform.GetChild(0).GetComponent<Volume>().enabled = GameDataManager.usePostProcessing;
    }

    void FixedUpdate()
    {
        if (Keyboard.current.rKey.IsPressed())
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene);
        }

        Gameplay();
    }

    void Gameplay()
    {
        if (!hitStop)
            UpdateEverything();
        else
        {
            if (hitStopTimer > 0)
                hitStopTimer--;
            else
            {
                hitStop = false;
                StopHitstop();
            }
        }

        scoreUI.text = "SCORE: " + score.ToString();
        GameDataManager.savedScore = score;
        GameDataManager.SaveToDisk();
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

                rbs.Remove(Characters[i].rb);
                Destroy(Characters[i].gameObject);
                Characters.RemoveAt(i);
            }
        }

        cameraClass.Update();
        boxClass.Update();
        waveManager.Update();
        robotClass.Update();
    }

    public void StartHitstop(float duration)
    {
        hitStopTimer = duration;
        hitStop = true;

        storedVelocities = new Vector2[rbs.Count];

        for (int i = 0; i < rbs.Count; i++)
        {
            if (rbs[i] == null) continue;
            storedVelocities[i] = rbs[i].linearVelocity;
            rbs[i].linearVelocity = Vector2.zero;
        }
    }

    void StopHitstop()
    {
        for (int i = 0; i < rbs.Count; i++)
        {
            if (rbs[i] == null) continue;
            rbs[i].linearVelocity = storedVelocities[i];
        }
    }

    public void Upgrade1()
    {
        Upgrades.SelectUpgrade(Upgrades.offeredUpgrades[0]);
    }

    public void Upgrade2()
    {
        Upgrades.SelectUpgrade(Upgrades.offeredUpgrades[1]);
    }

    public void Upgrade3()
    {
        Upgrades.SelectUpgrade(Upgrades.offeredUpgrades[2]);
    }

    public void GameOver()
    {
        if (score > GameDataManager.highscore)
        {
            GameDataManager.highscore = score;
            //new highscore
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameover");
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
    public GameObject gameObject;
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
    protected Vector2 spawnPoint = new Vector2(2, 0);
    public BoxBehaviour boxBehaviour;
    protected CollisionBehaviour collisionBehaviour = new CollisionBehaviour();
    public int lastSortingLayer;
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
        manager.rbs.Add(rb);
    }

    public override void Update()
    {
        base.Update();
        boxBehaviour.ClassUpdate();
    }
}

public class RobotClass : BaseClass
{
    public GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    public BoxCollider2D bc;
    public Rigidbody2D rb;
    protected Vector2 spawnPoint;
    public RobotBehaviour robotBehaviour;
    protected CollisionBehaviour collisionBehaviour = new CollisionBehaviour();

    public override void Start(Manager _manager)
    {
        base.Start(_manager);

        gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefab/Robot"));

        //Components
        sr = gameObject.GetComponent<SpriteRenderer>();
        robotBehaviour = gameObject.GetComponent<RobotBehaviour>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        robotBehaviour.bc = bc;
        robotBehaviour.rb = rb;
        robotBehaviour.manager = manager;
        robotBehaviour.player = manager.player;

        gameObject.transform.position = spawnPoint;
        gameObject.layer = manager.boxLayer;

        robotBehaviour.ClassStart();
    }

    public override void Update()
    {
        base.Update();
        robotBehaviour.ClassUpdate();
    }
}