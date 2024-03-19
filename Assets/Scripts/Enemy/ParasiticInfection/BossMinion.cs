using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BossMinion : EnemyController
{
    private Transform target;
    private bool isIdle = true;
    
    [Header("Boss Minion Settings"), Space]
    // Distance to stop between player and enemy
    public float stopDistance = 1f;

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
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance > stopDistance)
            {
                isIdle = false;
                Move();
            }
            else
            {
                isIdle = true;
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

    void Move()
    {
        MoveDirection();

        if (rb.velocity != Vector2.zero)
        {
            enemySprite.flipX = rb.velocity.x < 0; // flip sprite to direction of movement
            anim.SetBool("isWalking", true);
            EnemyStateMachine.TransitionTo(EnemyStateMachine.moveState);
        }
        else
        {
            anim.SetBool("isWalking", false);
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
            anim.SetBool("isWalking", false);
            EnemyStateMachine.TransitionTo(EnemyStateMachine.idleState);
        }
    }
    
}