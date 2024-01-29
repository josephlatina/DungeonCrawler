/*
 * RollState.cs
 * Author: Josh Coss
 * Created: January 23 2024
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
    /// Runs when first entering the move state
    /// </summary>
    public void Enter()
    {
        player.GetComponent<SpriteRenderer>().color = Color.red;
        // Add any additional logic needed when entering the roll state
        Roll();
    }

    /// <summary>
    /// Per-frame logic for the roll state - Include condition to transition to new state
    /// </summary>
    public void Update()
    {
        // Debug.Log("Player rolling");
        // Add roll-specific logic here
        RollCountdown();

        // Prevent player from attacking while rolling
        if (player.isMeleeAttacking || player.isRangedAttacking)
        {
            player.isMeleeAttacking = false;
            player.isRangedAttacking = false;
        }

        // Check conditions to transition to a different state
        if (player.rolling == false)
        {
            if (player.moveVal.x > 0.1f || player.moveVal.y > 0.1f)
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
            }
            else
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
            }
        }

    }

    public void FixedUpdate()
    {

    }

    /// <summary>
    /// Runs when exiting the move state
    /// </summary>
    public void Exit()
    {
        // Debug.Log("Player exiting roll state");
        player.GetComponent<SpriteRenderer>().color = Color.white;
        // Add any additional logic needed when exiting the roll state
        rollCounter = 0;
        rollCoolCounter = 0;
    }

    void Roll()
    {
        player.rb.velocity = player.moveVal * player.rollSpeed;
    }

    void RollCountdown()
    {
        if (rollCoolCounter <= 0 && rollCounter <= 0)
        {
            rollCounter = player.rollLength;
        }

        if (rollCounter > 0)
        {
            rollCounter -= Time.deltaTime;
            if (rollCounter <= 0)
            {
                rollCoolCounter = player.rollCooldown;
                player.rolling = false;
            }
        }
        if (rollCoolCounter > 0)
        {
            rollCoolCounter -= Time.deltaTime;
        }
    }
}
