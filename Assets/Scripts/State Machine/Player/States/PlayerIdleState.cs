/*
 * PlayerIdleState.cs
 * Author: Josh Coss
 * Created: January 16 2024
 * Description: Handles state transitions to and from the Idle state, as well as update logic for the state
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idle state logic for player
/// </summary>
public class PlayerIdleState : IState
{
    private PlayerController player;

    public PlayerIdleState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the idle state
    /// </summary>
    public void Enter()
    {

    }

    /// <summary>
    /// Per-frame logic for the idle state - Include condition to transition to new state
    /// </summary>
    public void Update()
    {
        // if we move above a minimum threshold, transition to move state
        if (player.moveVal.x != 0 || player.moveVal.y != 0)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
        }
        if (player.rolling)
        {
            player.rolling = false;
        }

        if (player.isMeleeAttacking)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.meleeState);
        }

        if (player.isRangedAttacking)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.rangedState);
        }
    }

    public void FixedUpdate()
    {

    }

    /// <summary>
    /// Runs when exiting the idle state
    /// </summary>
    public void Exit()
    {
        
    }
}
