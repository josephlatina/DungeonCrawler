using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : IState
{
    private EnemyController enemy;

    public EnemyMoveState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Enemy entering move state");
    }

    public void Update()
    {
        if (Mathf.Abs(enemy.rb.velocity.x) < 0.1f && Mathf.Abs(enemy.rb.velocity.y) < 0.1f)
        {
            enemy.EnemyStateMachine.TransitionTo(enemy.EnemyStateMachine.idleState);
        }
    }

    public void Exit()
    {
        Debug.Log("Enemy exiting move state");
    }
}
