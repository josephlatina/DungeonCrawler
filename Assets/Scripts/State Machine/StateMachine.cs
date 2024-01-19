/*
 * StateMachine.cs
 * Author: Josh Coss
 * Created: January 16 2024
 * Description: Manages how control flow enters and exits different states
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages state flow
/// </summary>
[Serializable]
public class StateMachine
{
    // The current state of the state machine
    public IState CurrentState { get; private set; }

    // Reference to the player's movement state
    public MoveState moveState;
    // Reference to the player's idle state
    public IdleState idleState;

    // Event to notify other objects of the state
    public event Action<IState> stateChanged;

    /// <summary>
    /// Constructor for player state machine
    /// </summary>
    /// <param name="player"></param>
    public StateMachine(PlayerController player)
    {
        // create an instance for each state and pass in PlayerController
        this.moveState = new MoveState(player);
        this.idleState = new IdleState(player);
    }

    /// <summary>
    /// Sets the initial state of the state machine
    /// </summary>
    /// <param name="state">The IState to initialize</param>
    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();

        // notify other objects that state has changed
        stateChanged?.Invoke(state);
    }

    /// <summary>
    /// Exit the current state and into another one
    /// </summary>
    /// <param name="nextState">The IState to transition into</param>
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();

        // notify other objects that state has changed
        stateChanged?.Invoke(nextState);
    }

    /// <summary>
    /// Allow the state machine to update the current state
    /// </summary>
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}
