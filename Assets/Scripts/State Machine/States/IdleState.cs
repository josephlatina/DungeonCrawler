using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private PlayerController player;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // code that runs when first entering the state
        Debug.Log("Entering Idle State");
    }

    // per-frame logic - include condition to transition to new state
    public void Update()
    {
        // if we move above a minimum threshold, transition to move state
        if (Mathf.Abs(player.rb.velocity.x) > 0.1f || Mathf.Abs(player.rb.velocity.y) > 0.1f)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
        }
    }

    public void Exit()
    {
        // code that runs when exiting the state
        Debug.Log("Exiting Idle State");
    }
}
