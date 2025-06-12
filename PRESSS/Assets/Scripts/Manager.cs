using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    List<character> Characters = new List<character>();

    void Start()
    {
        player player = new player();
        Characters.Add(player);

        foreach (character _character in Characters)
            _character.Start();
    }

    void Update()
    {
        foreach (character _character in Characters)
            _character.Update();
    }
}

public class character
{
    protected string name = "character";
    protected GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;

    public virtual void Start()
    {
        gameObject = new GameObject(name);
        gameObject.AddComponent<SpriteRenderer>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
    }

    public virtual void Update() { }
}

public class player : character
{
    public override void Start()
    {
        name = "Player";
        sprite = Resources.Load<Sprite>("Sprites/Player");
        base.Start();
    }

    public override void Update()
    {
        base.Update();

    }
}