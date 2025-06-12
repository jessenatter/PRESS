using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private InputSystem_Actions input;
    public Vector2 moveInput;
    List<character> Characters = new List<character>();

    void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        player player = new player();
        Characters.Add(player);

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
    protected GameObject gameObject;
    protected SpriteRenderer sr;
    protected Sprite sprite;
    protected BoxCollider2D bc;
    protected Rigidbody2D rb;
    protected float moveSpeed = 1f, lerpSpeed = 0.05f, deceleration = 0.1f;
    public Manager manager;
    protected Vector2 moveVec;

    public virtual void Start(Manager _manager)
    {
        manager = _manager;
        gameObject = new GameObject(name);
        sr = gameObject.AddComponent<SpriteRenderer>();
        bc = gameObject.AddComponent<BoxCollider2D>();
        rb = gameObject.AddComponent<Rigidbody2D>();
        sr.sprite = sprite;
        rb.gravityScale = 0;
    }

    public virtual void Update()
    {
        if (moveVec.x == 0 && moveVec.y == 0)
        {

        }
        else
        {
            rb.AddForce(moveVec * moveSpeed);
        }
    }
}

public class player : character
{
    public override void Start(Manager _manager)
    {
        name = "Player";
        sprite = Resources.Load<Sprite>("Sprites/Player");
        moveSpeed = 3f;
        base.Start(_manager);
    }

    public override void Update()
    {
        moveVec = manager.moveInput;
        base.Update();
    }
}