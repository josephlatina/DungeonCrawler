/*
 * PlayerController.cs
 * Author: Josh Coss
 * Created: January 16 2024
 * Description: Handle player movement and input
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    public Rigidbody2D rb;
    // Reference to player stats script
    private PlayerStats player;

    private float moveSpeed;
    // Vector2 of the normalized vectors of movement for the player
    private Vector2 moveVal;

    public PlayerStateMachine PlayerStateMachine => playerStateMachine;

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
        // Set current movement speed
        moveSpeed = player.currentMoveSpeed;
    }

    /// <summary>
    /// Called once after Awake
    /// </summary>
    private void Start()
    {
        // Set initial state to IdleState
        playerStateMachine.Initialize(playerStateMachine.idleState);

    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    private void Update()
    {
        // update the current state
        playerStateMachine.Update();
    }

    /// <summary>
    /// Called at fixed time intervals, making it suitable for physics-related calculations
    /// to ensure consistent behavior across varying frame rates.
    /// </summary>
    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Moves the player in all 8 directions
    /// </summary>
    private void Move()
    {
        rb.AddForce(new Vector2(moveVal.x * moveSpeed * Time.deltaTime, moveVal.y * moveSpeed * Time.deltaTime), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Listens for the player input and updates moveVal to the current value of movement input
    /// </summary>
    /// <param name="value">Movement vector sent from Player Input component</param>
    void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

}
