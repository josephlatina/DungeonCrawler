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

public class ConsumableItemController : InventoryItemController
{
    [Header("Item Settings"), Space]
    // Reference to the ScriptableObject of the Consumable Item type
    public ConsumableItem item;
    private InventorySystem playerInventory;
    private TextMeshProUGUI actionText;
    public SpriteRenderer sprite;

     void Start()
    {
        item.gameObject = gameObject; // reference current game object to scriptable object
        PlayerController playerController =
            GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerController>();
        playerInventory = playerController.playerInventory;
        actionText = playerController.text;
        
        // set the sorting layer
        sprite.sortingLayerName = "Instances";
        if (item.itemSprite)
        {
            sprite.sprite = item.itemSprite;
            sprite.color = Color.white;
        }
        
        UpdatePriceView(item);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        actionText.text = "";
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("interactTrigger"))
        {
            if (gameObject.tag == "interactableObject")
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
        }
    }

}