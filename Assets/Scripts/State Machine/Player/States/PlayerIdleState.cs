/*
 * PlayerIdleState.cs
 * Author: Josh Coss
 * Created: January 16, 2024
 * Description: Handles state transitions to and from the Idle state, as well as update logic for the state
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idle state logic for the player.
/// </summary>
public class PlayerIdleState : IState
{
    private PlayerController player;

    // Constructor to initialize the state with the associated player controller.
    public PlayerIdleState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the idle state.
    /// </summary>
    public void Enter()
    {
        // Code that runs when entering the idle state.
        player.anim.SetBool("isWalking", false);
    }

    /// <summary>
    /// Per-frame logic for the idle state - Include condition to transition to a new state.
    /// </summary>
    public void Update()
    {
        // If the player moves above a minimum threshold, transition to the move state.
        if (player.moveVal.x != 0 || player.moveVal.y != 0)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
        }

        // Check conditions to transition to other states based on player actions.
        if (player.isMeleeAttacking)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.meleeState);
        }

        if (player.isRangedAttacking)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.rangedState);
        }

        if (player.isHealing && !player.isMeleeAttacking && !player.isRangedAttacking && !player.isRolling && player.playerInventory.isConsumableFull())
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.healState);
        }
    }

    /// <summary>
    /// Fixed update logic for the idle state.
    /// </summary>
    public void FixedUpdate()
    {
        // This method is currently empty as there is no fixed update logic for the idle state.
    }

    /// <summary>
    /// Runs when exiting the idle state.
    /// </summary>
    public void Exit()
    {
        // Code that runs when exiting the idle state.
        // This method is currently empty.
    }
}
