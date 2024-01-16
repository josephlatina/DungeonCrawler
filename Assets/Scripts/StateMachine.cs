using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateMachine
{
    public IState CurrentState { get; private set; }

    // reference to the state objects
    public MoveState moveState;
    public IdleState idleState;

    // event to notify other objects of the state
    public event Action<IState> stateChanged;

    // pass in necessary parameters into constructor
    public StateMachine(PlayerController player)
    {
        // create an instance for each state and pass in PlayerController
        this.moveState = new MoveState(player);
        this.idleState = new IdleState(player);
    }

    // set the starting state
    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();

        // notify other objects that state has changed
        stateChanged?.Invoke(state);
    }

    // exit this state and enter another
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();

        // notify other objects that state has changed
        stateChanged?.Invoke(nextState);
    }

    // allow the StateMachine to update this state
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}
