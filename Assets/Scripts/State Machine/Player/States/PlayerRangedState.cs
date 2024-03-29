/*
 * PlayerRangedState.cs
 * Author: Josh Coss
 * Created: January 26 2024
 * Description: Handles state transitions to and from the Ranged state, as well as update logic for the state.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

/// <summary>
/// Ranged state logic for the player.
/// </summary>
public class PlayerRangedState : IState
{
    private PlayerController player;
    private Transform rangedWeapon;
    private PlayerStats stats;

    private float rangedCounter, rangedCoolCounter;

    // Constructor to initialize the state with the associated player controller.
    public PlayerRangedState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the ranged state.
    /// </summary>
    public void Enter()
    {
        // Get player's stats script.
        stats = player.GetComponent<PlayerStats>();
        if (player.weaponController.ranged)
        {
            rangedWeapon = player.weaponController.ranged.transform;
            // Initiate the ranged attack.
            Attack();
        }
    }

    /// <summary>
    /// Per-frame logic for the ranged state - Include condition to transition to a new state.
    /// </summary>
    public void Update()
    {
        // Perform ranged attack cooldown logic.
        RangedAttackCooldown();

        // Check conditions to transition to a different state.
        if (player.isRangedAttacking == false)
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
        // Move the player.
        player.Move();
    }

    /// <summary>
    /// Runs when exiting the ranged state.
    /// </summary>
    public void Exit()
    {

        // Reset counters.
        rangedCounter = 0;
        rangedCoolCounter = 0;
    }

    /// <summary>
    /// Manages the cooldown for the ranged attack.
    /// </summary>
    void RangedAttackCooldown()
    {
        if (rangedCoolCounter <= 0 && rangedCounter <= 0)
        {
            // Set the ranged counter when both counters reach zero.
            rangedCounter = 0.25f; // TODO: This will be replaced by the length of the attack animation.
        }

        if (rangedCounter > 0)
        {
            // Decrease the ranged counter and check for transition conditions.
            rangedCounter -= Time.deltaTime;
            if (rangedCounter <= 0)
            {
                // Initiate cooldown and end the ranged attack.
                rangedCoolCounter = stats.CurrentAttackSpeed; // Time between attacks.
                player.isRangedAttacking = false;
            }
        }
        if (rangedCoolCounter > 0)
        {
            // Decrease the cooldown counter.
            rangedCoolCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Initiates the ranged attack logic.
    /// </summary>
    void Attack()
    {
        // Enable the ranged trigger collider to detect hits.

        player.weaponController.FireRanged();

    }
}
