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

    public void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public void Update()
    {
        if (Mathf.Abs(player.CharController.velocity.x) < 0.1f && Mathf.Abs(player.CharController.velocity.y) < 0.1f)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Move State");
    }
}
