using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Manager : MonoBehaviour
{
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

// BLORB!