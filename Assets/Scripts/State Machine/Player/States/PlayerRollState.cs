/*
 * RollState.cs
 * Author: Josh Coss
 * Created: January 23 2024
 * Description: Handles state transitions to and from the Roll state, as well as update logic for the state
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Roll State Logic
/// </summary>
public class PlayerRollState : IState
{
    private PlayerController player;

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
        Debug.Log("Player entering roll state");
        // Add any additional logic needed when entering the roll state
    }

    /// <summary>
    /// Per-frame logic for the roll state - Include condition to transition to new state
    /// </summary>
    public void Update()
    {
        Debug.Log("Player rolling");
        // Add roll-specific logic here

        // Check conditions to transition to a different state
        if (player.rolling == false)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
        }
    }

    /// <summary>
    /// Runs when exiting the move state
    /// </summary>
    public void Exit()
    {
        Debug.Log("Player exiting roll state");
        // Add any additional logic needed when exiting the roll state
    }
}
