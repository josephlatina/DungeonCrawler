/*
 * PlayerMoveState.cs
 * Author: Josh Coss
 * Created: January 16 2024
 * Description: Handles state transitions to and from the Move state, as well as update logic for the state
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move state logic for player
/// </summary>
public class PlayerMoveState : IState
{
    private PlayerController player;

    // Constructor to initialize the state with the associated player controller
    public PlayerMoveState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the move state
    /// </summary>
    public void Enter()
    {
        // Debug.Log("Entering Move State");
    }

    /// <summary>
    /// Per-frame logic for the move state - Include condition to transition to new state
    /// </summary>
    public void Update()
    {
        // If movement stops, transition to idle state
        if (Mathf.Abs(player.rb.velocity.x) < 0.1f && Mathf.Abs(player.rb.velocity.y) < 0.1f)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
        if (player.rolling == true)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.rollState);
        }
    }

    /// <summary>
    /// Runs when exiting the move state
    /// </summary>
    public void Exit()
    {
        // code that runs when exiting the state
        // Debug.Log("Exiting Move State");
    }
}
