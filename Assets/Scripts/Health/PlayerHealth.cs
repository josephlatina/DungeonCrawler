using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        healthPoints = playerStats.CurrentHealth;
    }

    void Update()
    {

    }
}
