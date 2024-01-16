using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IState
{
    private PlayerController player;

    public MoveState(PlayerController player)
    {
        this.player = player;

    }

    // code that runs when first entering the state
    public void Enter()
    {
        Debug.Log("Entering Move State");
    }

    // per-frame logic - include condition to transition to new state
    public void Update()
    {
        // If movement stops, transition to idle state
        if (Mathf.Abs(player.rb.velocity.x) < 0.1f && Mathf.Abs(player.rb.velocity.y) < 0.1f)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
    }

    public void Exit()
    {
        // code that runs when exiting the state
        Debug.Log("Exiting Move State");
    }
}
