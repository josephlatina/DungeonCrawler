/*
 * ConsumableItem.cs
 * Author: Jehdi Aizon
 * Created: January 21, 2024
 * Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : InventoryItemBase
{
    [Header("Consumable Item Attributes")]
    [Space(5)]
    [Tooltip("How many hearts(1) or half hearts(0.5) the item restores. Value is 0 if ability doesn't exist")]
    [SerializeField] private float restoreHealthPoints;

    [Header("Extra Item Attributes")]
    // These attributes are randomly given to the item(weapons or consumable)
    [Space(5)]
    [Tooltip("Value is 0 if ability doesn't exist")]
    [SerializeField] private float abilityPoison;
    [Tooltip("Value is 0 if ability doesn't exist")]
    [SerializeField] private float abilityStun;
    [SerializeField] private float attackStrengthBoost;
    [SerializeField] private float defenceBoost;
    [SerializeField] private float movementSpeedBoost;
    [SerializeField] private float attackSpeedBoost;
}
