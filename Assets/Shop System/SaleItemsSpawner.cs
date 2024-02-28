using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SaleItemsSpawner : MonoBehaviour
{
    public List<Transform> itemsForSale;
    public GameObject consumableItemPrefab;
    public GameObject weaponItemPrefab;

    public List<ConsumableItem> spawnableConsumableItems;
    public List<WeaponItem> spawnableWeaponItems;

    // Start is called before the first frame update
    void Start()
    {
        // choose consumable/weapon list
        // choose random consumable/weapon
        // if consumable/weapaon get inventoryitemcontrol
        //
        
        foreach (Transform item in itemsForSale)
        {
            InventoryItemController itemControl = consumableItemPrefab.GetComponent<InventoryItemController>();
            itemControl.showPrice = true;
            
            ConsumableItem randItem = spawnableConsumableItems[Random.Range(0, spawnableConsumableItems.Count)];
            consumableItemPrefab.GetComponent<ConsumableItemController>().item = randItem;
            Debug.Log($"{randItem.itemName}");
            Instantiate(GetRandomItem(), item);
        }
    }

    GameObject GetRandomItem()
    {
        if (Random.Range(0,2) == 0)
        {
            InventoryItemController itemControl = consumableItemPrefab.GetComponent<InventoryItemController>();
            itemControl.showPrice = true;
            
            ConsumableItem randItem = spawnableConsumableItems[Random.Range(0, spawnableConsumableItems.Count)];
            consumableItemPrefab.GetComponent<ConsumableItemController>().item = randItem;
            return consumableItemPrefab;
        }
        else
        {
            InventoryItemController itemControl = weaponItemPrefab.GetComponent<InventoryItemController>();
            itemControl.showPrice = true;
            
            WeaponItem randItem = spawnableWeaponItems[Random.Range(0, spawnableWeaponItems.Count)];
            weaponItemPrefab.GetComponent<WeaponItemController>().item = randItem;
            return weaponItemPrefab;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}