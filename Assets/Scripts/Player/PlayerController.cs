/*
 * PlayerController.cs
 * Author: Josh Coss
 * Created: January 16 2024
 * Description: Handle player movement and input
 */
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input and movement
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    public Rigidbody2D rb;
    // Reference to player stats script
    private PlayerStats player;

    // Vector2 of the normalized vectors of movement for the player
    public Vector2 moveVal;
    public bool rolling;
    public float rollSpeed;
    public float rollLength = 0.5f, rollCooldown = 1f;

    public PlayerStateMachine PlayerStateMachine => playerStateMachine;
    [HideInInspector] public Collider2D meleeTrigger;
    private Collider2D interactTrigger;
    private Transform interactableObject;
    [HideInInspector] public Vector3 mousePos;

    private enum PlayerStatus
    {
        Normal,
        Stun,
        Immobilized,
        Poison
    }
    private PlayerStatus status;
    public bool isMeleeAttacking, isRangedAttacking;

    /// <summary>
    /// Called once when script is initialized
    /// </summary>
    private void Awake()
    {
        // Initialize state machine on player
        playerStateMachine = new PlayerStateMachine(this);
        // Get object's Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        // Get player object's stats script
        player = GetComponent<PlayerStats>();
        status = PlayerStatus.Normal;
        meleeTrigger = transform.Find("AimPivot/MeleeCollider").GetComponent<Collider2D>();
        interactTrigger = transform.Find("InteractTrigger").GetComponent<Collider2D>();
    }

    /// <summary>
    /// Called once after Awake
    /// </summary>
    private void Start()
    {
        // Initialize the state machine with the idle state
        playerStateMachine.Initialize(playerStateMachine.idleState);
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    private void Update()
    {
        // Update the state machine logic
        playerStateMachine.Update();
        // mousePos = aimPivot.mousePos;
    }

    /// <summary>
    /// Called at fixed time intervals, making it suitable for physics-related calculations
    /// to ensure consistent behavior across varying frame rates.
    /// </summary>
    private void FixedUpdate()
    {
        playerStateMachine.FixedUpdate();
    }

    /// <summary>
    /// Listens for the player input and updates moveVal to the current value of movement input
    /// </summary>
    /// <param name="value">Movement vector sent from Player Input component</param>
    void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

    /// <summary>
    /// Moves the player in all 8 directions
    /// </summary>
    public void Move()
    {
        rb.velocity = moveVal * player.currentMoveSpeed;
    }

    void OnRoll(InputValue value)
    {
        rolling = value.isPressed;
    }

    void OnMeleeAttack(InputValue value)
    {
        isMeleeAttacking = value.isPressed;
    }

    void OnRangedAttack(InputValue value)
    {
        isRangedAttacking = value.isPressed;
    }

    void OnInteract(InputValue value)
    {
        if (interactableObject)
        {
            // Debug.Log("You are interacting with " + interactableObject.name);
            Color newColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            interactableObject.GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "interactableObject" && interactableObject == null)
        {
            interactableObject = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "interactableObject" && interactableObject != null)
        {
            interactableObject = null;
        }
    }
}
