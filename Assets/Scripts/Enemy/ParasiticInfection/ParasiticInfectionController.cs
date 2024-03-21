using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

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
    private bool hasAttacked = false;

    private bool hasTalked = false;
    private bool dialogueComplete = false;
    [SerializeField] private YarnProject project;
    [YarnNode(nameof(project))] public string bossBattleNode;

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


            // conditions that would reset shooting attack
            if (Vector3.Distance(bullets.Last().transform.position, transform.position) > 20f)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    bullets[i].Reset();
                }

                // reset trigger
                hasShootAttacked = false;
            }

            // check if target is null
            target = target == null ? transform : target;

            float distance = Vector2.Distance(transform.position, target.position);
            // player is within detection range
            if (distance <= detectionRadius && !hasShootAttacked)
            {
                if (!hasTalked && distance <= detectionRadius - .5f)
                {
                    PlayerController player = target.GetComponent<PlayerController>();
                    player.anim.SetBool("isWalking", false);
                    player.paused = true;
                    player.dialogueRunner.StartDialogue(bossBattleNode);
                    player.dialogueRunner.onNodeComplete.AddListener(DialogueComplete);
                    hasTalked = true;
                    isIdle = true;
                }

                else if (dialogueComplete)
                {
                    if (!hasAttacked)
                    {
                        StartCoroutine(ReleaseMinion());
                        hasAttacked = true;
                    }
                    else
                    {
                        // within enemy's detection range
                        isIdle = true;
                        if (Time.time > nextFire)
                        {
                            nextFire = Time.time + bulletWaveInterval;
                            bullets[bulletIndex++].Shoot();

                            if (bulletIndex == bullets.Count)
                            {
                                bulletIndex = 0;
                                hasShootAttacked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                Move();
            }
        }
    }
    
    void DialogueComplete(string arg)
    {
        dialogueComplete = true;
        target.GetComponent<PlayerController>().paused = false;
    }

    IEnumerator ReleaseMinion()
    {
        anim.SetTrigger("isAttacking");
        yield return new WaitForSeconds(attackDelaySeconds);

        hasAttacked = false; // reset for next attack
    }

    /// <summary>
    /// Animation event for spawning minion at specific animation frame
    /// </summary>
    public void SpawnMinion()
    {
        Transform minion = Instantiate(minionObjectPrefab, transform).transform;
        minion.parent = null;
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
        if (!isIdle && dialogueComplete)
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