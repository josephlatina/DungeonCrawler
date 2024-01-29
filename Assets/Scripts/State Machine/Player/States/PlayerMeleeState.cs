/*
 * PlayerMeleeState.cs
 * Author: Josh Coss
 * Created: January 26 2024
 * Description: Handles state transitions to and from the Melee Attack state, as well as update logic for the state
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Melee Attack state logic for the player
/// </summary>
public class PlayerMeleeState : IState
{
    private PlayerController player;
    private PlayerStats stats;

    private float meleeCounter, meleeCoolCounter;

    // Constructor to initialize the state with the associated player controller
    public PlayerMeleeState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the Melee Attack state
    /// </summary>
    public void Enter()
    {
        // Change player's color to blue when entering the Melee Attack state.
        player.GetComponent<SpriteRenderer>().color = Color.blue;
        // Get player's stats component.
        stats = player.GetComponent<PlayerStats>();
        // Initiate the melee attack logic.
        Attack();
    }

    /// <summary>
    /// Per-frame logic for the Melee Attack state - Include condition to transition to a new state
    /// </summary>
    public void Update()
    {
        // Manage Melee Attack cooldown.
        MeleeAttackCooldown();

        // Check conditions to transition to a different state.
        if (player.isMeleeAttacking == false)
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

    /// <summary>
    /// Runs at fixed time intervals, making it suitable for physics-related calculations.
    /// </summary>
    public void FixedUpdate()
    {
        // Move the player during the Melee Attack state.
        player.Move();
    }

    /// <summary>
    /// Runs when exiting the Melee Attack state
    /// </summary>
    public void Exit()
    {
        // Reset player's color to white when exiting the Melee Attack state.
        player.GetComponent<SpriteRenderer>().color = Color.white;
        // Disable the melee trigger collider.
        player.meleeTrigger.enabled = false;

        // Reset counters.
        meleeCounter = 0;
        meleeCoolCounter = 0;
    }

    /// <summary>
    /// Manages the cooldown for Melee Attack and the time between attacks.
    /// </summary>
    void MeleeAttackCooldown()
    {
        if (meleeCoolCounter <= 0 && meleeCounter <= 0)
        {
            // Set the Melee Attack counter when both counters reach zero.
            meleeCounter = 0.25f; // TODO: this will be replaced by the length of the attack animation
        }

        if (meleeCounter > 0)
        {
            // Decrease the Melee Attack counter and check for timer reaching 0.
            meleeCounter -= Time.deltaTime;
            if (meleeCounter <= 0)
            {
                // Initiate cooldown and end the Melee Attack.
                meleeCoolCounter = stats.CurrentAttackSpeed; // Time between attacks
                player.isMeleeAttacking = false;
            }
        }
        if (meleeCoolCounter > 0)
        {
            // Decrease the cooldown counter.
            meleeCoolCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Initiates the Melee Attack.
    /// </summary>
    void Attack()
    {
        // Enable the melee trigger collider to detect hits.
        player.meleeTrigger.enabled = true;
    }
}
