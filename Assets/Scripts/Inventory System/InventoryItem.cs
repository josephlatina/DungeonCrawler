/*
 * InventoryItem.cs
 * Author: Jehdi Aizon,
 * Created: January 21, 2024
 * Description:
 */
using UnityEngine;

public abstract class InventoryItem : ScriptableObject
{
    public string itemName;
    [TextArea(1, 5)] public string description;
    public int price;

    // Add actions that are common in all items
    // but treated differently based on type to an item below 
    public abstract void Add();
    // TODO: ADD MORE
}