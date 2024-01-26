/*
 * EnemyMoveState.cs
 * Author: Josh Coss
 * Created: January 20 2024
 * Description: Represents the state when the enemy is actively moving.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the state when the enemy is actively moving.
/// </summary>
public class EnemyMoveState : IState
{
    private EnemyController enemy;

    /// <summary>
    /// Constructor to initialize the state with the associated enemy controller.
    /// </summary>
    public EnemyMoveState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// Called when entering the move state.
    /// </summary>
    public void Enter()
    {
        Debug.Log("Enemy entering move state");
    }

    /// <summary>
    /// Called every frame while in the move state.
    /// Checks if the enemy's velocity is low; if yes, transitions to the idle state.
    /// </summary>
    public void Update()
    {
        // Check if the enemy's velocity is below a certain threshold; if yes, transition to idle state
        if (Mathf.Abs(enemy.rb.velocity.x) < 0.1f && Mathf.Abs(enemy.rb.velocity.y) < 0.1f)
        {
            enemy.EnemyStateMachine.TransitionTo(enemy.EnemyStateMachine.idleState);
        }
    }

    public void FixedUpdate()
    {

    }

    /// <summary>
    /// Called when exiting the move state.
    /// </summary>
    public void Exit()
    {
        Debug.Log("Enemy exiting move state");
    }
}
