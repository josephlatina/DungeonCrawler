using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * InventoryItemController.cs
 * Author: Jehdi Aizon
 * Created: February 20, 2024
 * Description: Base class for Consumable and Weapon Items
 */
public class InventoryItemController : MonoBehaviour
{
    protected PlayerController playerController;

    [Header("Shop System"), Space] public bool showPrice = false;
    public GameObject priceView;
    public bool itemLocked;
    [SerializeField] protected TextMesh priceText;

    public void UpdatePriceView(InventoryItem item, bool isLocked)
    {
        priceView.SetActive(showPrice); // show price above item

        if (showPrice)
        {
            priceText.text = $"{item.price}";
        }

        if (isLocked)
        {
            priceText.color = Color.red;
        }
    }
}