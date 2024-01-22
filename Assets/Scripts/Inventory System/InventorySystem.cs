/*
 * InventorySystem.cs
 * Author: Jehdi Aizon
 * Created: January 18, 2024
 * Description: Houses the actions used in an Inventory System. Must be a Scriptable Object
 */

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventorySystem", menuName = "DungeonCrawler/New Inventory", order = 0)]

public class InventorySystem : ScriptableObject
{
    // List of InventoryItem ScriptableObjects
    public List<InventoryItem> items;

    public void AddItem(InventoryItem item)
    {
        items.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        items.Remove(item);
    }

    public void DisplayInventory()
    {
        Debug.Log($"\nINVENTORY=======");
        foreach (var item in items)
        {
            Debug.Log($"Item: {item.itemName}");
        }
    }

    public void Reset()
    {
        items.Clear();
    }

    
}