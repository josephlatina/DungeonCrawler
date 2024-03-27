/*
 * EnemyIdleState.cs
 * Author: Josh Coss
 * Created: January 20 2024
 * Description: Defines the idle state behavior for an enemy in a state machine.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the idle state of an enemy in a state machine
/// </summary>
public class EnemyIdleState : IState
{
    private EnemyController enemy;

    // Constructor to initialize the state with the associated enemy controller
    public EnemyIdleState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    // Called when entering the idle state
    public void Enter()
    {
        //Debug.Log("Enemy entering idle state");
        enemy.anim.SetBool("isWalking", false);
    }

    // Called every frame while in the idle state
    public void Update()
    {
        // Check if there is significant movement; if yes, transition to the move state
        if (Mathf.Abs(enemy.rb.velocity.x) > 0f || Mathf.Abs(enemy.rb.velocity.y) > 0f)
        {
            enemy.EnemyStateMachine.TransitionTo(enemy.EnemyStateMachine.moveState);
        }
    }

    public void FixedUpdate()
    {

    }

    // Called when exiting the idle state
    public void Exit()
    {
        // Debug.Log("Enemy exiting idle state");
    }

}
