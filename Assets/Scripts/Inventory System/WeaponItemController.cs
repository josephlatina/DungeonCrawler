/*
 * ConsumableItemController.cs
 * Author: Jehdi Aizon
 * Created: January 22, 2024
 * Description: This is the script that attaches to the weapon prefab.
 * It houses the scriptable object of a Weapon Item.
 */

using UnityEngine;

public class WeaponItemController : MonoBehaviour
{
    // Reference to the ScriptableObject of the Weapon Item type
    public WeaponItem item;

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
            gameObject.SetActive(false); // hides object from scene
        }
    }
}