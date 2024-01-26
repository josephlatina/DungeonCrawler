/*
 * BaseStateMachine.cs
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
public class BaseStateMachine
{
    // The current state of the state machine
    public IState CurrentState { get; private set; }

    // Event to notify other objects of the state
    public event Action<IState> stateChanged;

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

    public void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.FixedUpdate();
        }
    }
}
