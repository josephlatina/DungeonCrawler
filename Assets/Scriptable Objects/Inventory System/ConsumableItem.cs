/*
 * ConsumableItem.cs
 * Author: Jehdi Aizon
 * Created: January 21, 2024
 * Description: scriptable object that determines stats and attributes of consumable item
 */

using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Inventory System/New Consumable Item")]
public class ConsumableItem : InventoryItem
{
    // Any value that is 0, ability doesn't exist or no effect

    [Header("Basic Consumable Attributes")]
    [Space(5)]
    [Tooltip("How many hearts(1) or half hearts(0.5) the item restores. Value is 0 if ability doesn't exist")]
    public float healthRestore;

    [Header("Permanent Power Upgrade Attributes (PERK)")] [Space(5)]
    public float attackStrengthUpgrade;

    public float attackSpeedUpgrade;
    public int defenceUpgrade;

    // These attributes are randomly given to the item(weapons or consumable)
    [Header("Extra Item Attributes")] [Space(5)] [Tooltip("Tiles enemy is pushed back")]
    public int knockback;

    [Tooltip("Health points drained per attack by chance")]
    public float abilityPoison;

    [Tooltip("Health points drained per attack by chance")]
    public float abilityStun;

    public float attackStrengthBoost;
    public float defenceBoost;
    public float movementSpeedBoost;
    public float attackSpeedBoost;
}