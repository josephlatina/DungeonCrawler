/*
 * RollState.cs
 * Author: Josh Coss
 * Created: January 23, 2024
 * Description: Handles state transitions to and from the Roll state, as well as update logic for the state
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Roll State Logic
/// </summary>
public class PlayerRollState : IState
{
    private PlayerController player;

    private float rollCounter;
    private float rollCoolCounter;

    // Constructor to initialize the state with the associated player controller
    public PlayerRollState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the roll state
    /// </summary>
    public void Enter()
    {
        // Change player's color to red when entering the roll state.
        player.GetComponent<SpriteRenderer>().color = Color.red;
        // Initiate the roll logic.
        Roll();
    }

    /// <summary>
    /// Per-frame logic for the roll state - Include condition to transition to new state
    /// </summary>
    public void Update()
    {
        // Add roll-specific logic here.
        RollCountdown();

        // Check conditions to transition to a different state.
        if (player.isRolling == false)
        {
            if (player.moveVal.x > 0.1f || player.moveVal.y > 0.1f)
            {
                // Transition to move state if the player is moving.
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
            }
            else
            {
                // Transition to idle state if the player is not moving.
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
            }
        }
    }

    public void FixedUpdate()
    {
        // This method is currently empty as there is no fixed update logic for the roll state.
    }

    /// <summary>
    /// Runs when exiting the roll state
    /// </summary>
    public void Exit()
    {
        // Reset player's color to white when exiting the roll state.
        player.GetComponent<SpriteRenderer>().color = Color.white;
        // Set counters back to 0
        rollCounter = 0;
        rollCoolCounter = 0;
    }

    /// <summary>
    /// Initiates the roll movement.
    /// </summary>
    void Roll()
    {
        player.rb.velocity = player.moveVal * player.rollSpeed;
    }

    /// <summary>
    /// Manages the countdown for the roll and cooldown durations.
    /// </summary>
    void RollCountdown()
    {
        if (rollCoolCounter <= 0 && rollCounter <= 0)
        {
            // Set the roll counter when both counters reach zero.
            rollCounter = player.rollLength;
        }

        if (rollCounter > 0)
        {
            // Decrease the roll counter and check to see if counter has finished.
            rollCounter -= Time.deltaTime;
            if (rollCounter <= 0)
            {
                // Initiate cooldown and end the roll.
                rollCoolCounter = player.rollCooldown;
                player.isRolling = false;
            }
        }
        if (rollCoolCounter > 0)
        {
            // Decrease the cooldown counter.
            rollCoolCounter -= Time.deltaTime;
        }
    }
}
