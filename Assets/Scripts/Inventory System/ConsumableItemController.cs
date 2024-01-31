/*
 * ConsumableItemController.cs
 * Author: Jehdi Aizon
 * Created: January 21, 2024
 * Description: This is the script that attaches to the consumable prefab.
 * It houses the scriptable object of a Consumable Item.
 * Determines behaviour of consumable items.
 */

using System;
using TMPro;
using UnityEngine;

public class ConsumableItemController : MonoBehaviour
{
    // Reference to the ScriptableObject of the Consumable Item type
    public ConsumableItem item;
    private InventorySystem playerInventory;
    private TextMeshProUGUI actionText;
    
    private void Start()
    {
        item.gameObject = gameObject; // reference current game object to scriptable object
        Debug.Log($"{item.name} {item.description} {item.price}");
        PlayerController playerController =
            GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerController>();
        playerInventory = playerController.playerInventory;
        actionText = playerController.text;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // check if consumable slots are full replace, if not pick up
        if (playerInventory.isConsumableFull())
        {
            actionText.text = $"Press E to replace consumable item with {item.itemName}";
        }
        else
        {
            actionText.text = $"Press E to pick up consumable item {item.itemName}";
        }
    }
    
    public void DropItemAt(Vector2 dropPosition)
    {
        item.gameObject.transform.position = dropPosition;
        item.gameObject.SetActive(true);
    }
}