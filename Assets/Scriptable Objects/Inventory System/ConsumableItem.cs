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
    [SerializeField]
    private float healthRestore;

    [Header("Permanent Power Upgrade Attributes (PERK)")] [Space(5)] [SerializeField]
    private float attackStrengthUpgrade;

    [SerializeField] private float attackSpeedUpgrade;
    [SerializeField] private float defenceUpgrade;

    // These attributes are randomly given to the item(weapons or consumable)
    [Header("Extra Item Attributes")] [Space(5)] [Tooltip("Tiles enemy is pushed back")] [SerializeField]
    private int knockback;

    [Tooltip("Health points drained per attack by chance")] [SerializeField]
    private float abilityPoison;

    [Tooltip("Health points drained per attack by chance")] [SerializeField]
    private float abilityStun;

    [SerializeField] private float attackStrengthBoost;
    [SerializeField] private float defenceBoost;
    [SerializeField] private float movementSpeedBoost;
    [SerializeField] private float attackSpeedBoost;

    public override void Add()
    {
        // Implement the specific behavior for adding consumable
        Debug.Log($"Adding {itemName}");
    }
}