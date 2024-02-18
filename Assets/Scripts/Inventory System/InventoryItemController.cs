using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryItemController : MonoBehaviour
{
    [Header("Shop System"), Space]
    public bool showPrice = false;
    public List<Sprite> numberSprites;
    public GameObject numberPrefab;
    protected InventoryItem itemScriptObject;
    public GameObject priceView;
    public Transform numbersContainer;

    public void UpdatePriceView(InventoryItem item)
    {
        foreach (char num in item.price.ToString())
        {
            int number = num - '0';
            numberPrefab.GetComponent<SpriteRenderer>().sprite = numberSprites[number];
            Instantiate(numberPrefab, numbersContainer);
        }
    }
}