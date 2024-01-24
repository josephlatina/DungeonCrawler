/*
 * WeaponItem.cs
 * Author: Jehdi Aizon,
 * Created: January 18, 2024
 * Description:
 */

using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Inventory System/New Weapon Type", order = 0)]
public class WeaponItem : InventoryItem
{
    // Any value that is 0, ability doesn't exist or no effect
    
    [Header("Basic Weapon Attributes")] [Space(5)] [SerializeField]
    private float damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int attackRange;
    [SerializeField] private float criticalHitDamage;
    [Tooltip("Tiles enemy is pushed back")]
    [SerializeField] private int knockback;

    [Header("Specific Weapon Abilities")] [Space(5)]
    [Tooltip("Health points drained per attack by chance")] [SerializeField]
    private int abilityLifeSteal;

    [Tooltip("Health points drained per attack by chance")] 
    [SerializeField]
    private float abilityPoison;

    [Tooltip("Value is 0 if ability doesn't exist")]     
    [SerializeField]
    private float abilityStun;

    [Header("Extra Item Attributes")]
    // These attributes are randomly given to the item(weapons or consumable) 
    [Space(5)]
    [SerializeField]
    private float attackStrengthBoost;

    [SerializeField] private float defenceBoost;
    [SerializeField] private float movementSpeedBoost;
    [SerializeField] private float attackSpeedBoost;

    
    public override void Add()
    {
        // Implement the specific behavior for adding weapon
        Debug.Log($"Adding {itemName}");
    }
}