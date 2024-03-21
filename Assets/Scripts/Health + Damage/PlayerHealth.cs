using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float currentHealthPoints;
    [SerializeField] public float maxHealthPoints;
    [SerializeField] private int defence;
    [SerializeField] private float incomingDamage;
    public static event Action OnHealthChanged;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerController playerController;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        maxHealthPoints = playerStats.MaxHealthPoints;
        currentHealthPoints = maxHealthPoints;
        defence = playerStats.CurrentDefence;
    }

    public void ChangeHealth(float healthChange)
    {
        currentHealthPoints += healthChange;
        if (healthChange < 0) {
            playerController.Hurt();
        }
        OnHealthChanged?.Invoke();

        if (currentHealthPoints <= 0)
        {
            playerController.Death();
        }

        if (currentHealthPoints > maxHealthPoints)
        {
            currentHealthPoints = maxHealthPoints;
        }
    }
}
