using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : BaseStateMachine
{

    // Reference to the player's movement state
    public PlayerMoveState moveState;
    // Reference to the player's idle state
    public PlayerIdleState idleState;


    /// <summary>
    /// Constructor for player state machine
    /// </summary>
    /// <param name="player"></param>
    public PlayerStateMachine(PlayerController player)
    {
        // create an instance for each state and pass in PlayerController
        this.moveState = new PlayerMoveState(player);
        this.idleState = new PlayerIdleState(player);
    }
}
