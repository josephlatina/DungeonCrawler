/*
 * EnemyController.cs
 * Author: Jehdi Aizon,
 * Created: January 17, 2024
 * Description: Handles enemy controller movements.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Scriptable Object")]
    [Tooltip("EnemyScriptableObject for initial enemy stats")]
    [SerializeField]
    private EnemyScriptableObject enemyStats;

    private EnemyStateMachine enemyStateMachine;
    [HideInInspector]
    public Rigidbody2D rb;

    // movement speed of character
    protected float movementSpeed;
    // measured in damage per second
    private float attackSpeed;
    // amount of hearts character inflicts to other (full heart or half heart)
    private float strength;
    // amount of heart a character has
    private float healthPoints;
    // state of enemy
    private enum enemyStatus { };

    public EnemyStateMachine EnemyStateMachine => enemyStateMachine;

    public virtual void Awake()
    {
        enemyStateMachine = new EnemyStateMachine(this);
        rb = GetComponentInChildren<Rigidbody2D>();

        movementSpeed = enemyStats.MovementSpeed;
        attackSpeed = enemyStats.AttackSpeed;
        strength = enemyStats.Strength;
        healthPoints = enemyStats.HealthPoints;
    }

    public virtual void Start()
    {
        enemyStateMachine.Initialize(enemyStateMachine.idleState);
    }

    public virtual void Update()
    {
        enemyStateMachine.Update();
    }
}
