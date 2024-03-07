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
public class EnemyController : MonoBehaviour, IEffectable
{
    [Header("Enemy Scriptable Object")]
    [Tooltip("EnemyScriptableObject for initial enemy stats")]
    [SerializeField]
    private EnemyScriptableObject enemyStats;

    private EnemyStateMachine enemyStateMachine;
    [HideInInspector] public Rigidbody2D rb;

    // Movement speed of the enemy
    protected float movementSpeed;

    // Attack speed measured in damage per second
    private float attackSpeed;

    // Strength: amount of damage inflicted by the enemy
    public float strength;

    // Health points of the enemy
    // private float currentHealthPoints;
    protected EnemyHealth health;
    public float maxHealth;

    // Status effect of the enemy
    private enum EnemyStatus
    {
        Normal,
        Stun,
        Immobilized,
        Poison
    };
    // private EnemyStatus status;

    public StatusEffectData data;

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
        maxHealth = enemyStats.HealthPoints;

        health = GetComponent<EnemyHealth>();

        // status = EnemyStatus.Normal;
    }

    /// <summary>
    /// Called once after Awake.
    /// </summary>
    public virtual void Start()
    {
        // Initialize the state machine with the idle state
        enemyStateMachine.Initialize(enemyStateMachine.idleState);
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public virtual void Update()
    {
        // Update the state machine logic
        enemyStateMachine.Update();
        if (data != null)
        {
            HandleEffect();
        }
    }

    /// <summary>
    /// Initialize the enemy
    /// </summary>
    public void EnemyInitialization(EnemyScriptableObject enemySO, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyStats = enemySO;
    }

    public float GetMaxHealthPoints()
    {
        return maxHealth;
    }

    private GameObject effectParticles;
    public void ApplyEffect(StatusEffectData data)
    {
        this.data = data;
        if (data.EffectParticles)
        {
            effectParticles = Instantiate(data.EffectParticles, GetComponentInChildren<EnemyDamage>().transform);
        }
    }

    private float currentEffectTime = 0f;
    private float lastTickTime = 0f;
    public void RemoveEffect()
    {
        data = null;
        currentEffectTime = 0;
        lastTickTime = 0;
        if (effectParticles != null)
        {
            Destroy(effectParticles);
        }
    }

    public void HandleEffect()
    {
        currentEffectTime += Time.deltaTime;

        if (currentEffectTime >= data.Lifetime)
        {
            RemoveEffect();
        }

        if (data == null) return;

        if (data.DamageOverTimeAmount != 0 && currentEffectTime > lastTickTime + data.TickSpeed)
        {
            lastTickTime = currentEffectTime;
            health.ChangeHealth(-data.DamageOverTimeAmount);
        }
    }
}
