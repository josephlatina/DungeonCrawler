using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryItemController : MonoBehaviour
{
    public List<Sprite> sprites;
    public GameObject numberPrefab;
    protected InventoryItem itemScriptObject;
    public bool showPrice = false;
    public GameObject priceView;
    public Transform numbersContainer;

    public void UpdatePriceView(InventoryItem item)
    {
        foreach (char num in item.price.ToString())
        {
            int number = num - '0';
            numberPrefab.GetComponent<SpriteRenderer>().sprite = sprites[number];
            Instantiate(numberPrefab, numbersContainer);
        }
    }
}