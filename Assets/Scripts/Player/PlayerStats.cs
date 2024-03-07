/*
 * PlayerStats.cs
 * Author: Josh Coss
 * Created: January 18, 2024
 * Description: Handles the storing and manipulation of the player's stats
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the storing and manipulation of the player's stats.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    // Represents the player's maximum health
    [SerializeField] private float maxHealthPoints;

    // Represents the player's current health
    [SerializeField] private float currentHealthPoints;

    // Represents the player's current movement speed
    [SerializeField] private float currentMoveSpeed;

    // Represents the player's current speed of attack
    [SerializeField] private float currentAttackSpeed;

    // Represents the player's current attack strength
    [SerializeField] private float currentStrength;

    // Represents the player's current defence
    [SerializeField] private int currentDefence;

    // Represents the player's current incoming damage
    [SerializeField] private float currentIncomingDamage;

    /// <summary>
    /// Getter and setter for MaxHealthPoints.
    /// </summary>
    public float MaxHealthPoints
    {
        get { return maxHealthPoints; }
        set
        {
            // Check if the value has changed
            if (maxHealthPoints != value)
            {
                maxHealthPoints = value;
            }
        }
    }

    /// <summary>
    /// Getter and setter for CurrentHealthPoints.
    /// </summary>
    public float CurrentHealthPoints
    {
        get { return currentHealthPoints; }
        set
        {
            // Check if the value has changed
            if (currentHealthPoints != value)
            {
                currentHealthPoints = value;
            }
        }
    }

    /// <summary>
    /// Getter and setter for CurrentMoveSpeed.
    /// </summary>
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            // Check if the value has changed
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
            }
        }
    }

    /// <summary>
    /// Getter and setter for CurrentAttackSpeed.
    /// </summary>
    public float CurrentAttackSpeed
    {
        get { return currentAttackSpeed; }
        set
        {
            // Check if the value has changed
            if (currentAttackSpeed != value)
            {
                currentAttackSpeed = value;
            }
        }
    }

    /// <summary>
    /// Getter and setter for CurrentStrength.
    /// </summary>
    public float CurrentStrength
    {
        get { return currentStrength; }
        set
        {
            // Check if the value has changed
            if (currentStrength != value)
            {
                currentStrength = value;
            }
        }
    }

    /// <summary>
    /// Getter and setter for CurrentDefence.
    /// </summary>
    public int CurrentDefence
    {
        get { return currentDefence; }
        set
        {
            // Check if the value has changed
            if (currentDefence != value)
            {
                currentDefence = value;
            }
        }
    }

    /// <summary>
    /// Getter and setter for CurrentIncomingDamage.
    /// </summary>
    public float CurrentIncomingDamage
    {
        get { return currentIncomingDamage; }
        set
        {
            // Check if the value has changed
            if (currentIncomingDamage != value)
            {
                currentIncomingDamage = value;
            }
        }
    }
}
