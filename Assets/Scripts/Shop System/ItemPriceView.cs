using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemPriceView : MonoBehaviour
{
    public List<GameObject> spawnableItems;

    public int amountOfItemsForSale = 3;
    // public GameObject item;
    //
    // public GameObject price;
    
    // public TextMeshProUGUI priceText;
    public Transform priceView;

    public InventoryItemController inventoryItemController;

    // Start is called before the first frame update
    void Start()
    {
        // TODO behaviour of multiple buyable items
        // numberView.price = inventoryItemController.GetPrice();
        // Add random item from spawnableItems list
        for (int i = 0; i < amountOfItemsForSale; i++)
        {
            // Instantiate(itemForSale, transform, true);
            // Transform parentTransform = priceView.GetChild(i);
            //
            // // Instantiate(ChooseRandomItem(), itemForSale.transform, true);
            // GameObject item = Instantiate(ChooseRandomItem(), parentTransform);
            // item.AddComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    GameObject ChooseRandomItem()
    {
        return spawnableItems[Random.Range(0, spawnableItems.Count)];
    }
}