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
    [Header("Shop System"), Space] public bool showPrice = false;
    public GameObject priceView;

    protected void UpdatePriceView(InventoryItem item)
    {
        priceView.SetActive(showPrice); // show price above item
        if (showPrice)
        {
            TextMesh priceText = priceView.GetComponent<TextMesh>();
            priceText.GetComponent<Renderer>().sortingLayerName = "Instances";
            priceText.text = $"{item.price}";
        }
    }
}