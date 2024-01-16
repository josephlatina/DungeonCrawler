using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private StateMachine playerStateMachine;
    public Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    public Vector2 moveVal;

    public StateMachine PlayerStateMachine => playerStateMachine;


    private void Awake()
    {
        // initialize state machine
        playerStateMachine = new StateMachine(this);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        playerStateMachine.Initialize(playerStateMachine.idleState);
    }

    private void Update()
    {
        // update the current state
        playerStateMachine.Update();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.AddForce(new Vector2(moveVal.x * moveSpeed * Time.deltaTime, moveVal.y * moveSpeed * Time.deltaTime), ForceMode2D.Impulse);
    }


    void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

}
