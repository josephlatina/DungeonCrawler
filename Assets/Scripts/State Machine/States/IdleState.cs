/*
 * IdleState.cs
 * Author: Josh Coss
 * Created: January 16 2024
 * Description: Handles state transitions to and from the Idle state, as well as update logic for the state
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idle state logic
/// </summary>
public class IdleState : IState
{
    private PlayerController player;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the idle state
    /// </summary>
    public void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    /// <summary>
    /// Per-frame logic for the idle state - Include condition to transition to new state
    /// </summary>
    public void Update()
    {
        // if we move above a minimum threshold, transition to move state
        if (Mathf.Abs(player.rb.velocity.x) > 0.1f || Mathf.Abs(player.rb.velocity.y) > 0.1f)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
        }
    }

    /// <summary>
    /// Runs when exiting the idle state
    /// </summary>
    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
