using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Update = UnityEngine.PlayerLoop.Update;

public class SaleItemsSpawner : MonoBehaviour
{
    public List<Transform> itemsForSale;
    public GameObject consumableItemPrefab;

    public List<ConsumableItem> spawnableConsumableItems;
    public List<GameObject> spawnableWeaponItems;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform item in itemsForSale)
        {
            InventoryItemController randItem = GetRandomItem().GetComponent<InventoryItemController>();
            randItem.tag = "onSaleItem";
            Instantiate(randItem.gameObject, item);
        }
    }

    GameObject GetRandomItem()
    {
        // 0: consumable, 1: weapon
        if (Random.Range(0, 2) == 0)
        {
            // choose random consumable from given
            ConsumableItem randItem = spawnableConsumableItems[Random.Range(0, spawnableConsumableItems.Count)];
            consumableItemPrefab.GetComponent<ConsumableItemController>().item = randItem;

            return consumableItemPrefab;
        }
        else
        {
            // choose random weapon from given
            GameObject randItem = spawnableWeaponItems[Random.Range(0, spawnableWeaponItems.Count)];

            WeaponItemController itemControl = randItem.GetComponent<WeaponItemController>();

            return itemControl.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}