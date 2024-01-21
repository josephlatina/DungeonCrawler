/*
 * PlayerController.cs
 * Author: Jehdi Aizon
 * Created: January 18, 2024
 * Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItemBase : MonoBehaviour, IInventoryItem
{
    public string itemName;
    public string Name => itemName;

    public string itemDescription;
    public string Description => itemDescription;

    public int itemPrice;
    public int Price => itemPrice;
}