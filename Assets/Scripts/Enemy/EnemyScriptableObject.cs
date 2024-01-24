/*
 * PlayerController.cs
 * Author: Josh Coss
 * Created: January 16 2024
 * Description: Scriptable Object for Enemy
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define a ScriptableObject for an Enemy
/// </summary>
[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    // Serialized field for movement speed
    [SerializeField]
    float movementSpeed;

    // Property for accessing movement speed with private setter
    public float MovementSpeed { get => movementSpeed; private set => movementSpeed = value; }

    // Serialized field for attack speed
    [SerializeField]
    float attackSpeed;

    // Property for accessing attack speed with private setter
    public float AttackSpeed { get => attackSpeed; private set => attackSpeed = value; }

    // Serialized field for strength
    [SerializeField]
    float strength;

    // Property for accessing strength with private setter
    public float Strength { get => strength; private set => strength = value; }

    // Serialized field for health points
    [SerializeField]
    float healthPoints;

    // Property for accessing health points with private setter
    public float HealthPoints { get => healthPoints; private set => healthPoints = value; }
}
