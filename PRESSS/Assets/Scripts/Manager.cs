using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class Manager : MonoBehaviour
{
    List<Character> Characters = new List<Character>();
    List<BaseClass> BaseClasses = new List<BaseClass>();

    int enemyCount = 1;
    public Player player = new Player();
    CameraClass cameraClass = new CameraClass();
    public BoxClass boxClass = new BoxClass();

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
        BaseClasses.Add(boxClass);

        foreach (BaseClass _baseClass in BaseClasses)
            _baseClass.Start(this);
    }

    void Update()
    {
        foreach (Character _character in Characters)
            _character.Update();

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
    protected BoxCollider2D bc;
    protected Rigidbody2D rb;
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

        gameObject.transform.position = spawnPoint;
    }

    public override void Update()
    {

    }
}