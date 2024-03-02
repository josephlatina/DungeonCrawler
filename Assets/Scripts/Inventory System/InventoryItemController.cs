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

    [Header("Inventory Item Shop System"), Space]
    public bool showPrice = false;

    public GameObject priceView;
    public bool itemLocked = false;
    [SerializeField] protected TextMesh priceText;
    [HideInInspector] public InventoryItem inventoryItem;

    protected virtual void Start()
    {
        priceText.text = $"{inventoryItem.price}";
    }

    /// <summary>
    /// Check if player has enough currency to purchase.
    /// </summary>
    /// <param name="color">color used on text when item is not buyable</param>
    public void UpdateText(Color color)
    {
        if (playerController.player.CurrentCurrency < inventoryItem.price)
        {
            priceText.color = color;
        }
        else
        {
            priceText.color = Color.white;
        }
    }

    /// <summary>
    /// Toggle price view
    /// </summary>
    /// <param name="showPriceView"></param>
    public void ShowPriceView(bool showPriceView)
    {
        showPrice = showPriceView;
        priceView.SetActive(showPriceView);
    }

    protected virtual void Update()
    {
        if (CompareTag("onSaleItem"))
        {
            UpdateText(Color.red);
            ShowPriceView(true);
        }
        else
        {
            itemLocked = false;
            ShowPriceView(false);
        }
    }
}