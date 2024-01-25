/*
 * ConsumableItemController.cs
 * Author: Jehdi Aizon
 * Created: January 21, 2024
 * Description: This is the script that attaches to the consumable prefab.
 * It houses the scriptable object of a Consumable Item.
 */

using UnityEngine;

public class ConsumableItemController : MonoBehaviour
{
    // Reference to the ScriptableObject of the Consumable Item type
    public ConsumableItem item;

    private void Start()
    {
        Debug.Log($"{item.name} {item.description} {item.price}");
    }

    private void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InventorySystem playerInventory = other.gameObject.GetComponent<PlayerController>().playerInventory;

            playerInventory.AddItem(item);
            playerInventory.DisplayInventory();
        }
    }
}