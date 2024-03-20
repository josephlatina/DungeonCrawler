using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParasiticInfectionController : EnemyController
{
    private Transform target;
    private bool isIdle = true;

    [Header("Parasitic Boss Settings"), Space]
    public GameObject minionObjectPrefab;

    public List<Bullet> bullets;
    private int bulletIndex = 0;
    private float nextFire = 0f;

    public float detectionRadius = 5f;
    public float attackDelaySeconds = 2f;
    public float bulletWaveInterval = 5f;
    private float nextWave = 0f;
    private bool hasShootAttacked = false;

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
            EnemyStateMachine.TransitionTo(EnemyStateMachine.idleState);
        }
        else
        {
            EnemyStateMachine.TransitionTo(EnemyStateMachine.moveState);
        }

       

        if (Vector3.Distance(bullets.Last().transform.position, transform.position) > 20f)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Reset();
            }
            hasShootAttacked = false;

        }

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= detectionRadius && !hasShootAttacked)
        {
            // within enemy's detection range
            isIdle = true;
            if (Time.time > nextFire)
            {
                nextFire = Time.time + attackDelaySeconds;
                bullets[bulletIndex++].Shoot();
                if (bulletIndex == bullets.Count)
                {
                    bulletIndex = 0;
                    hasShootAttacked = false;
            
                }
            }
        }
        else
        {
            Move();
        }
    }


    void Move()
    {
        isIdle = false;
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

    private void OnDrawGizmosSelected()
    {
        // Set the color of the wireframe circle
        Gizmos.color = Color.red;

        // Draw wireframe circle
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}