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
    public static event Action OnPlayerDamaged;

    [SerializeField] private PlayerStats playerStats;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        maxHealthPoints = playerStats.MaxHealthPoints;
        currentHealthPoints = maxHealthPoints - 1.5f;
    }

    public void ChangeHealth(float healthChange)
    {
        currentHealthPoints += healthChange;
        OnPlayerDamaged?.Invoke();

        if (currentHealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
