using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeState : IState
{
    private EnemyController enemy;
   

    private float meleeCounter, meleeCoolCounter;

    // Constructor to initialize the state with the associated player controller
    public EnemyMeleeState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// Runs when first entering the Melee Attack state
    /// </summary>
    public void Enter()
    {
        Debug.Log("Enemy entering melee state");
        enemy.anim.SetBool("isAttacking", true);
    }

    /// <summary>
    /// Per-frame logic for the Melee Attack state - Include condition to transition to a new state
    /// </summary>
    public void Update()
    {
        if (Vector3.Distance(enemy.transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) > 10f)
        {
            enemy.EnemyStateMachine.TransitionTo(enemy.EnemyStateMachine.moveState);
        }
    }

    public void FixedUpdate()
    {
        
    }

    /// <summary>
    /// Runs when exiting the Melee Attack state
    /// </summary>
    public void Exit()
    {
        Debug.Log("Enemy exiting melee state");
        enemy.anim.SetBool("isAttacking", false);
    }


    /// <summary>
    /// Initiates the Melee Attack.
    /// </summary>
    void Attack()
    {
        enemy.anim.SetBool("isAttacking", true);
    }
}
