/*
 * InventorySystem.cs
 * Author: Jehdi Aizon
 * Created: January 18, 2024
 * Description: Houses the actions used in an Inventory System. Must be a Scriptable Object
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    // List of InventoryItem ScriptableObjects
    public List<InventoryItem> items;
    public int maxWeaponSlots = 2;
    public int maxConsumableSlots = 3;
    [SerializeField] private GridLayoutGroup inventoryGridLayoutGroup;

    private void Update()
    {
        DisplayInventory();
    }

    public void AddItem(InventoryItem item)
    {
        items.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        items.Remove(item);
    }

    /// <summary>
    /// Swap what is in items[index] with item
    /// </summary>
    /// <param name="item">item replacing current</param>
    /// <param name="index">position at which to replace</param>
    public void SwapItemAt(InventoryItem item, int index)
    {
        items[index] = item;
    }

    public InventoryItem GetItemAt(int index)
    {
        return items[index];
    }

    public bool isConsumableFull()
    {
        return items.OfType<ConsumableItem>().Count() == maxConsumableSlots - 1;
    }

    public ConsumableItem GetConsumable()
    {
        if (isConsumableFull())
        {
            return (ConsumableItem)GetItemAt(2);
        }
        return null;
    }

    public bool isWeaponFull()
    {
        return items.OfType<WeaponItem>().Count() == maxWeaponSlots;
    }

    public bool isMeleeWeaponFull()
    {
        return items[0] != null;
    }

    public bool isRangeWeaponFull()
    {
        return items[1] != null;
    }

    // Watch https://www.youtube.com/watch?v=oJAE6CbsQQA
    public void DisplayInventory()
    {
        for (int i = 0; i < items.Count(); i++)
        {
            GameObject itemSlot = inventoryGridLayoutGroup.transform.GetChild(i).transform.GetChild(0).gameObject;
            // Debug.Log(itemSlot);
            if (items[i])
            {
                itemSlot.GetComponent<Image>().sprite = items[i].itemSprite;
            }
            else if (!items[i])
            {
                itemSlot.GetComponent<Image>().sprite = null;
            }
        }
    }

    public void InitializeInventory()
    {
        for (int i = 0; i < maxConsumableSlots + maxConsumableSlots; i++)
        {
            AddItem(null);
        }
    }

    public void Reset()
    {
        items.Clear();
    }
}