using System;
using UnityEngine;

public class ParasiticInfectionController : EnemyController
{
    public Transform target;
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
        Debug.Log(EnemyStateMachine.CurrentState);

        if (health.currentHealthPoints <= 0)
        {
            anim.SetBool("isDead", true);
        }
        else
        {
            Move();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == target)
        {
            isIdle = true;
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == target)
        {
            isIdle = false;
        }
    }
}