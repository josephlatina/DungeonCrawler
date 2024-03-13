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
    private SpriteRenderer enemySprite;

    private EnemyStateMachine enemyStateMachine;
    [HideInInspector] public Rigidbody2D rb;

    // Movement speed of the enemy
    public float movementSpeed;
    public float currentMovementSpeed;

    // Attack speed measured in damage per second
    private float attackSpeed;

    // Strength: amount of damage inflicted by the enemy
    public float strength;

    // Health points of the enemy
    // private float currentHealthPoints;
    protected EnemyHealth health;
    public float maxHealth;
    private bool paused;

    // Status effect of the enemy
    // private enum EnemyStatus
    // {
    //     Normal,
    //     Stun,
    //     Immobilized,
    //     Poison
    // };
    // private EnemyStatus status;

    public StatusEffectData effectOnEnemy;
    public StatusEffectData effectEnemyApplies;

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

        enemySprite = GetComponentInChildren<SpriteRenderer>();

        // Set the enemy's stats based on the EnemyScriptableObject
        movementSpeed = enemyStats.MovementSpeed;
        attackSpeed = enemyStats.AttackSpeed;
        strength = enemyStats.Strength;
        maxHealth = enemyStats.HealthPoints;

        currentMovementSpeed = movementSpeed;

        health = GetComponent<EnemyHealth>();
        initialSpriteColor = enemySprite.color;
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
        if (!paused)
        {
            // Update the state machine logic
            enemyStateMachine.Update();
        }
        if (effectOnEnemy != null)
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
    private Color initialSpriteColor;
    public void ApplyEffect(StatusEffectData data)
    {
        RemoveEffect();
        effectOnEnemy = data;

        if (effectOnEnemy)
        {
            if (data.poisonParticles)
            {
                effectParticles = Instantiate(data.poisonParticles, GetComponentInChildren<EnemyDamage>().transform);
            }
            if (data.effectName == "Health Steal")
            {
                GameObject.FindWithTag("Player").GetComponent<PlayerHealth>().ChangeHealth(0.5f);
            }
            if (data.effectName == "Immobilized")
            {
                currentMovementSpeed = data.movementPenalty;
                enemySprite.color = data.immobilizedEffect;
            }
            if (data.effectName == "Stun")
            {
                enemySprite.color = data.stunEffect;
                //prevAnimSpeed = anim.speed;
                //anim.speed = 0;
                paused = true;
            }
        }
    }

    private float currentEffectTime = 0f;
    private float lastTickTime = 0f;
    public void RemoveEffect()
    {
        currentEffectTime = 0;
        lastTickTime = 0;
        if (effectOnEnemy)
        {
            if (effectOnEnemy.movementPenalty != -1)
            {
                currentMovementSpeed = enemyStats.MovementSpeed;
            }
            if (effectOnEnemy.effectName == "Immobilized" || effectOnEnemy.effectName == "Stun")
            {
                enemySprite.color = initialSpriteColor;
                paused = false;
            }
            if (effectOnEnemy.effectName == "Stun")
            {
                //anim.speed = prevAnimSpeed;
            }
        }
        if (effectParticles != null)
        {
            Destroy(effectParticles);
        }
        effectOnEnemy = null;
    }

    public void HandleEffect()
    {
        currentEffectTime += Time.deltaTime;

        if (currentEffectTime >= effectOnEnemy.lifetime)
        {
            RemoveEffect();
        }

        if (effectOnEnemy == null) return;

        if (effectOnEnemy.damageOverTimeAmount != 0 && currentEffectTime > lastTickTime + effectOnEnemy.tickSpeed)
        {
            lastTickTime = currentEffectTime;
            health.ChangeHealth(-effectOnEnemy.damageOverTimeAmount);
        }
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
