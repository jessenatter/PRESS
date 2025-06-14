using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class Manager : MonoBehaviour
{
    public List<Character> Characters = new List<Character>();
    List<BaseClass> BaseClasses = new List<BaseClass>();

    int enemyCount = 3;
    public Player player = new Player();
    CameraClass cameraClass = new CameraClass();
    public BoxClass boxClass = new BoxClass();
    public int playerLayer, boxLayer, enemyLayer, wallLayer;
    public LayerMask playerMask, boxMask, enemyMask, wallMask;

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

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

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

    public override void Start(Manager _manager)
    {
        gameObject = GameObject.FindGameObjectWithTag("MainCamera");
        base.Start(_manager);
    }

    public override void Update()
    {
        base.Update();
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
    }
}