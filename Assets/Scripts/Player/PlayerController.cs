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

    private float moveSpeed;
    private float attackSpeed;
    private float strength;
    private float healthPoints;
    private int defence;
    private float incomingDamage;
    // Vector2 of the normalized vectors of movement for the player
    public Vector2 moveVal;
    public bool rolling;
    public float rollSpeed;
    public float rollLength = 0.5f, rollCooldown = 1f;
    private float rollCounter;
    private float rollCoolCounter;

    public PlayerStateMachine PlayerStateMachine => playerStateMachine;

    private enum PlayerStatus
    {
        Normal,
        Stun,
        Immobilized,
        Poison
    }
    private PlayerStatus status;

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

        // Set player's stats based on player stats script
        moveSpeed = player.currentMoveSpeed;
        attackSpeed = player.currentAttackSpeed;
        strength = player.currentStrength;
        healthPoints = player.currentHealth;
        defence = player.currentDefence;
        incomingDamage = player.currentIncomingDamage;
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

        if (rolling)
        {
            if (rollCoolCounter <= 0 && rollCounter <= 0)
            {
                rollCounter = rollLength;
            }
        }
        if (rollCounter > 0)
        {
            rollCounter -= Time.deltaTime;
            if (rollCounter <= 0)
            {
                rollCoolCounter = rollCooldown;
                rolling = false;
            }
        }
        if (rollCoolCounter > 0)
        {
            rollCoolCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Called at fixed time intervals, making it suitable for physics-related calculations
    /// to ensure consistent behavior across varying frame rates.
    /// </summary>
    private void FixedUpdate()
    {
        if (!rolling)
        {
            Move();
        }
    }

    /// <summary>
    /// Moves the player in all 8 directions
    /// </summary>
    public void Move()
    {
        rb.velocity = moveVal * moveSpeed;
    }

    public void Roll()
    {
        rb.velocity = moveVal * rollSpeed;
    }

    /// <summary>
    /// Listens for the player input and updates moveVal to the current value of movement input
    /// </summary>
    /// <param name="value">Movement vector sent from Player Input component</param>
    void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

    void OnRoll(InputValue value)
    {
        if (rollCoolCounter <= 0 && rollCounter <= 0)
        {
            rolling = true;
            Roll();
        }
    }
}
