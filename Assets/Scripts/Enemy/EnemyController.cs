/*
 * EnemyController.cs
 * Authors: Jehdi Aizon, Josh Coss
 * Created: January 17, 2024
 * Description: Handles base enemy controller movements.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the basic behavior of an enemy, such as movement and state transitions.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Scriptable Object")]
    [Tooltip("EnemyScriptableObject for initial enemy stats")]
    [SerializeField]
    public EnemyScriptableObject enemyStats;

    private EnemyStateMachine enemyStateMachine;
    [HideInInspector] public Rigidbody2D rb;

    // Movement speed of the enemy
    protected float movementSpeed;
    // Enemy Movement AI component references
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;

    // Attack speed measured in damage per second
    private float attackSpeed;

    // Strength: amount of damage inflicted by the enemy
    public float strength;

    // Health points of the enemy
    private float healthPoints;

    // Status effect of the enemy
    private enum EnemyStatus
    {
        Normal,
        Stun,
        Immobilized,
        Poison
    };
    private EnemyStatus status;

    public Vector2 knockbackVelocity;
    public float knockbackDuration;

    // Expose the EnemyStateMachine for external access
    public EnemyStateMachine EnemyStateMachine => enemyStateMachine;

    /// <summary>
    /// Called once when the script is initialized.
    /// </summary>
    public virtual void Awake()
    {
        // Initialize the state machine on the enemy
        enemyStateMachine = new EnemyStateMachine(this);

        // Get the object's Rigidbody2D component
        rb = GetComponentInChildren<Rigidbody2D>();

        // Set the enemy's stats based on the EnemyScriptableObject
        movementSpeed = enemyStats.MovementSpeed;
        attackSpeed = enemyStats.AttackSpeed;
        strength = enemyStats.Strength;
        healthPoints = enemyStats.HealthPoints;

        status = EnemyStatus.Normal;

        // Cache the enemy movement components
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    /// <summary>
    /// Called once after Awake.
    /// </summary>
    public virtual void Start()
    {
        // Initialize the state machine with the idle state
        enemyStateMachine.Initialize(enemyStateMachine.idleState);
    }

    protected virtual void FixedUpdate()
    {

    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public virtual void Update()
    {
        // Update the state machine logic
        enemyStateMachine.Update();
    }

    /// <summary>
    /// Initialize the enemy
    /// </summary>
    public void EnemyInitialization(EnemyScriptableObject enemySO, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyStats = enemySO;
    }

    /// <summary>
    /// Getter method for movement speed
    /// </summary>
    public float GetMoveSpeed()
    {
        return movementSpeed;
    }

    public float GetHealthPoints()
    {
        return healthPoints;
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0)
        {
            return;
        }
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
