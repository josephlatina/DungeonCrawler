using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BossMinion : EnemyController
{
    [Header("Boss Minion Settings"), Space]

    // Distance to stop between player and enemy
    public float stopDistance = 1f;

    public float detectionRadius = 5f;
    public float attackDelaySeconds = 2f;
    private bool hasAttacked = false;

    private Transform target;
    private bool isIdle = true;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        target = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (health.currentHealthPoints <= 0)
        {
            anim.SetBool("isDead", true);
        }
        else
        {
            // Update velocity based on boolean
            if (isIdle)
            {
                rb.velocity = Vector2.zero;
                EnemyStateMachine.TransitionTo(EnemyStateMachine.idleState);
            }
            else
            {
                EnemyStateMachine.TransitionTo(EnemyStateMachine.moveState);
            }
            
            // check if target is null
            target = target == null ? transform : target;
            
            // Move enemy towards player when in detection radius
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance <= detectionRadius)
            {
                if (distance <= stopDistance)
                {
                    isIdle = true;
                }
                else
                {
                    Move();
                }

                if (!hasAttacked && Time.time >= attackDelaySeconds)
                {
                    isIdle = true;
                    StartCoroutine(Attack());
                    hasAttacked = true; // attack has been executed 
                }
            }
            else
            {
                isIdle = true;
            }
        }
    }
    
    IEnumerator Attack()
    {
        anim.SetTrigger("isAttacking");

        yield return new WaitForSeconds(attackDelaySeconds / 2);

        hasAttacked = false; // reset for next attack
    }

    /// <summary>
    /// Function to handle animation event when attack animation is done
    /// </summary>
    public void AttackAnimationFinished()
    {
        anim.ResetTrigger("isAttacking");
    }

    /// <summary>
    /// Animation event to attack the player at specific frame
    /// </summary>
    public void AttackPlayer()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        PlayerController player = target.GetComponent<PlayerController>();

        // if distance from player is stopDistance + 1 while on attack frame, take damage 
        if (distance <= stopDistance + 1 && player.playerHealth.currentHealthPoints > 0)
        {
            player.UpdatePlayerStats(health: -strength);
        }
    }

    void Move()
    {
        isIdle = false;
        MoveDirection();

        if (rb.velocity != Vector2.zero)
        {
            enemySprite.flipX = rb.velocity.x < 0; // flip sprite to direction of movement
            EnemyStateMachine.TransitionTo(EnemyStateMachine.moveState);
        }
        else
        {
            EnemyStateMachine.TransitionTo(EnemyStateMachine.idleState);
        }
    }

    void MoveDirection()
    {
        if (!isIdle)
        {
            // Calculate the direction from current position to the target
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;

            // Move the Rigidbody2D towards the target using velocity
            rb.velocity = direction * currentMovementSpeed;
        }
        else
        {
            EnemyStateMachine.TransitionTo(EnemyStateMachine.idleState);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Set the color of the wireframe circle
        Gizmos.color = Color.red;

        // Draw wireframe circle
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}