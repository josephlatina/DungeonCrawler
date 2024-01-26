/*
 * PlayerStateMachine.cs
 * Author: Josh Coss
 * Created: January 20 2024
 * Description: Represents the state machine for controlling player behavior.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the state machine for controlling player behavior.
/// </summary>
public class PlayerStateMachine : BaseStateMachine
{
    // Reference to the player's movement state
    public PlayerMoveState moveState;
    // Reference to the player's idle state
    public PlayerIdleState idleState;
    public PlayerRollState rollState;
    public PlayerMeleeState meleeState;
    public PlayerRangedState rangedState;

    /// <summary>
    /// Constructor for the player state machine.
    /// </summary>
    /// <param name="player">The associated player controller.</param>
    public PlayerStateMachine(PlayerController player)
    {
        // Create an instance for each state and pass in the PlayerController
        this.moveState = new PlayerMoveState(player);
        this.idleState = new PlayerIdleState(player);
        this.rollState = new PlayerRollState(player);
        this.meleeState = new PlayerMeleeState(player);
        this.rangedState = new PlayerRangedState(player);
    }
}
