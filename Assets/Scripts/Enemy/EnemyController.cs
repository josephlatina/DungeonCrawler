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
    [SerializeField]
    private EnemyScriptableObject enemyStats;

    private EnemyStateMachine enemyStateMachine;
    public Rigidbody2D rb;

    // movement speed of character
    [SerializeField] private float movementSpeed;
    // measured in damage per second
    [SerializeField] private float attackSpeed;
    // amount of hearts character inflicts to other (full heart or half heart)
    [SerializeField] private float strength;
    // amount of heart a character has
    [SerializeField] private float healthPoints;
    // state of enemy
    [SerializeField] private enum enemyStatus { };

    public EnemyStateMachine EnemyStateMachine => enemyStateMachine;

    void Awake()
    {
        enemyStateMachine = new EnemyStateMachine(this);
        rb = GetComponentInChildren<Rigidbody2D>();

        movementSpeed = enemyStats.MovementSpeed;
        attackSpeed = enemyStats.AttackSpeed;
        strength = enemyStats.Strength;
        healthPoints = enemyStats.HealthPoints;
    }

    void Start()
    {
        enemyStateMachine.Initialize(enemyStateMachine.idleState);
    }

    private void Update()
    {
        enemyStateMachine.Update();
    }
}
