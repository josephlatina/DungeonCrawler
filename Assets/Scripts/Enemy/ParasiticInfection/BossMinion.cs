using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BossMinion : EnemyController
{
    [Header("Boss Minion Settings"), Space]
    private Transform target;

    private bool isIdle = true;

    // Distance to stop between player and enemy
    public float stopDistance = 1f;
    public float attackDelaySeconds = 2f;
    private bool hasAttacked = false;

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

        // Update velocity based on boolean
        if (isIdle)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform == target)
        {
            isIdle = true;

            float distance = Vector2.Distance(transform.position, target.position);
            if (distance > stopDistance)
            {
                isIdle = false;
                Move();
            }
            else
            {
                isIdle = true;
                
                // hasAttacked ensures 1 attack
                // attackDelaySeconds ensures cooldown between attacks
                if (!hasAttacked && Time.time >= attackDelaySeconds)
                {
                    StartCoroutine(Attack());
                    hasAttacked = true; // attack has been executed 
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == target)
        {
            isIdle = true;
        }
    }

    IEnumerator Attack()
    {
        anim.SetTrigger("isAttacking");

        yield return new WaitForSeconds(1f);

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
        
        // if distance from player is stopDistance + 1 while on attack frame, take damage 
        if (distance <= stopDistance + 1)
        {
            target.GetComponent<PlayerController>().UpdatePlayerStats(health: -strength);
        }
    }

    void Move()
    {
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
}