/*
 * PlayerStats.cs
 * Author: Josh Coss
 * Created: January 18 2024
 * Description: Handles the storing and manipulation of the player's stats
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores and manipulates the player's stats
/// </summary>
public class PlayerStats : MonoBehaviour
{
    // Represents the player's current health
    [SerializeField] float CurrentHealth;
    // Represents the player's current movement speed
    [SerializeField] float CurrentMoveSpeed;
    // Represents the player's current speed of attack
    [SerializeField] float CurrentAttackSpeed;
    // Represents the player's current attack strength
    [SerializeField] float CurrentStrength;
    [SerializeField] int CurrentDefence;
    [SerializeField] float IncomingDamage;

    // Setter / getter for CurrentHealth
    public float currentHealth
    {
        get { return CurrentHealth; }
        set
        {
            //Check if the value has changed
            if (CurrentHealth != value)
            {
                CurrentHealth = value;
            }
        }
    }

    // Setter / getter for CurrentMoveSpeed
    public float currentMoveSpeed
    {
        get { return CurrentMoveSpeed; }
        set
        {
            if (CurrentMoveSpeed != value)
            {
                CurrentMoveSpeed = value;
            }
        }
    }

    // Setter / getter for CurrentAttackSpeed
    public float currentAttackSpeed
    {
        get { return CurrentAttackSpeed; }
        set
        {
            if (CurrentAttackSpeed != value)
            {
                CurrentAttackSpeed = value;
            }
        }
    }

    // Setter / getter for CurrentStrength
    public float currentStrength
    {
        get { return CurrentStrength; }
        set
        {
            if (CurrentStrength != value)
            {
                CurrentStrength = value;
            }
        }
    }

    // Setter / getter for CurrentDefence
    public int currentDefence
    {
        get { return CurrentDefence; }
        set
        {
            if (CurrentDefence != value)
            {
                CurrentDefence = value;
            }
        }
    }

    // Setter / getter for CurrentIncomingDamage
    public float CurrentIncomingDamage
    {
        // TODO look at replacement
        //  get { return CurrentIncomingDamage; }
        get { return IncomingDamage;}
        set
        {
            if (CurrentIncomingDamage != value)
            {
                CurrentIncomingDamage = value;
            }
        }
    }

}
