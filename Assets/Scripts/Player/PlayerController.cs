using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private StateMachine playerStateMachine;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [Tooltip("Rate of change for move speed")]
    [SerializeField] private float acceleration = 10f;

    public CharacterController CharController => charController;
    public StateMachine PlayerStateMachine => playerStateMachine;

    private CharacterController charController;
    private float targetSpeed;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        charController = GetComponent<CharacterController>();

        // initialize state machine
        playerStateMachine = new StateMachine(this);
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

    private void LateUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 inputVector = playerInput.InputVector;

        if (inputVector == Vector2.zero)
        {
            targetSpeed = 0;
        }

        float currentSpeed = new Vector2(charController.velocity.x, charController.velocity.y).magnitude;
        float tolerance = 0.1f;

        if (currentSpeed < targetSpeed - tolerance! || currentSpeed > targetSpeed + tolerance)
        {
            targetSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);
            targetSpeed = Mathf.Round(targetSpeed * 1000f) / 1000f;
        }
        else
        {
            targetSpeed = moveSpeed;
        }

        charController.Move(inputVector.normalized * targetSpeed * Time.deltaTime);
    }

}
