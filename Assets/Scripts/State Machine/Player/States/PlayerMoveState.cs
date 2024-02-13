/*
 * PlayerMoveState.cs
 * Author: Josh Coss
 * Created: January 16, 2024
 * Description: Handles state transitions to and from the Move state, as well as update logic for the state
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move state logic for the player.
/// </summary>
public class PlayerMoveState : IState
{
    private PlayerController player;

    // Constructor to initialize the state with the associated player controller.
    public PlayerMoveState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the move state.
    /// </summary>
    public void Enter()
    {
        // Code that runs when entering the move state.
        player.anim.SetBool("isWalking", true);
    }

    /// <summary>
    /// Per-frame logic for the move state - Include condition to transition to new state.
    /// </summary>
    public void Update()
    {

        // If movement stops, transition to idle state.
        if (Mathf.Abs(player.rb.velocity.x) < 0.1f && Mathf.Abs(player.rb.velocity.y) < 0.1f)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }

        // Check conditions to transition to other states based on player actions.
        if (player.isRolling && !player.isMeleeAttacking && !player.isRangedAttacking)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.rollState);
        }

        if (player.isMeleeAttacking && !player.isRolling && !player.isRangedAttacking)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.meleeState);
        }

        if (player.isRangedAttacking)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.rangedState);
        }

        if (player.isHealing)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.healState);
        }
    }

    /// <summary>
    /// Fixed update logic for the move state.
    /// </summary>
    public void FixedUpdate()
    {
        // Call the Move method from PlayerController.
        player.Move();
    }

    /// <summary>
    /// Runs when exiting the move state.
    /// </summary>
    public void Exit()
    {
        // Code that runs when exiting the move state.

    }
}
