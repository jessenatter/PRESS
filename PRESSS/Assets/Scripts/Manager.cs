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

    int enemyCount = 1;
    public Player player = new Player();
    public CameraClass cameraClass = new CameraClass();
    public BoxClass boxClass = new BoxClass();
    public int playerLayer, boxLayer, enemyLayer, wallLayer, score;
    public LayerMask playerMask, boxMask, enemyMask, wallMask;
    public bool hitStop = false;
    float hitStopDuration = 15, hitStopTimer,waveStartDuration = 100,waveStartTimer;

    bool WaveLoaded = false,StartedToLoadWave = false;
    int wave;

    [SerializeField] protected TextMeshProUGUI scoreUI, waveUI;
    Vector2 enemySpawnPoint = new Vector2(-7.5f, 2.5f);

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

        foreach (BaseClass _baseClass in BaseClasses)
            _baseClass.Start(this);
    }

    void FixedUpdate()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        if (!WaveLoaded)
            LoadNextWave();

        Gameplay();
    }

    void Gameplay()
    {
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

        scoreUI.text = "SCORE: " + score.ToString();
    }

    void LoadNextWave()
    {
        if(!StartedToLoadWave)
        {
            //activate UI 
            waveUI.enabled = true;
            wave += 1;
            waveUI.text = "WAVE: " + wave.ToString();
            StartedToLoadWave = true;
            enemyCount = Mathf.RoundToInt(wave * Random.Range(1,1.75f));
        }
        else
        {
            //wait a bit and then spawn enemies
            waveStartTimer++;
            if(waveStartTimer == waveStartDuration)
            {
                WaveLoaded = true;
                waveStartTimer = 0;

                int leftSideSpawns = 0, rightSideSpawns = 0;
                float yPos = 0;

                for (int i = 0; i < enemyCount; i++)
                {
                    Enemy enemy = new Enemy();
                    int spawnSide = Random.value < 0.5f ? 1 : -1;

                    if(spawnSide == 1)
                    {
                        yPos = enemySpawnPoint.y - (2 * leftSideSpawns);
                        leftSideSpawns++;
                    }
                    else
                    {
                        yPos = enemySpawnPoint.y - (2 * rightSideSpawns);
                        rightSideSpawns++;
                    }

                    
                    enemy.spawnPoint = new Vector2(enemySpawnPoint.x * spawnSide,yPos);
                    Characters.Add(enemy);
                    enemy.Start(this);
                    waveUI.enabled = false;
                }
            }
        }
    }

    void UpdateEverything()
    {
        int currentEnemyCount = 0;

        for (int i = Characters.Count - 1; i >= 0; i--)
        {
            if (Characters[i] is Enemy)
                currentEnemyCount++;

            Characters[i].Update();
            if (Characters[i].dead)
            {
                Destroy(Characters[i].gameObject);
                Characters.RemoveAt(i);
            }
        }

        if (currentEnemyCount == 0 && WaveLoaded)
        {
            WaveLoaded = false;
            StartedToLoadWave = false;
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
    float screenShakeTimer, screenshakeDuration = 20, shakeFrequency = 1, shakeMagnitude = 0.1f;
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
            gameObject.transform.position = new Vector3(initPos.x + shake, initPos.y + shake,initPos.z);

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