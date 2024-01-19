/*
 * PlayerController.cs
 * Author: Jehdi Aizon
 * Created: January 18, 2024
 * Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : InventoryItemBase
{
    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int attackRange;
    [SerializeField] private float criticalHitDamage;
    [SerializeField] private int knockback;
    [SerializeField] private int abilityLifeSteal;
    [SerializeField] private float abilityPoison;
    [SerializeField] private float abilityStun;

    public void Weapon(string name, float damage, float attackSpeed, int attackRange, float criticalHitDamage, int knockback, int abilityLifeSteal, float abilityPoison, float abilityStun)
    {
        itemName = name;
        this.damage = damage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
        this.criticalHitDamage = criticalHitDamage;
        this.knockback = knockback;
        this.abilityLifeSteal = abilityLifeSteal;
        this.abilityPoison = abilityPoison;
        this.abilityStun = abilityStun;
    }
}
