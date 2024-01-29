/*
 * ConsumableItemController.cs
 * Author: Jehdi Aizon
 * Created: January 22, 2024
 * Description: This is the script that attaches to the weapon prefab.
 * It houses the scriptable object of a Weapon Item.
 * Determines behaviour of weapon items.
 */

using TMPro;
using UnityEngine;

public class WeaponItemController : MonoBehaviour
{
    // Reference to the ScriptableObject of the Weapon Item type
    public WeaponItem item;
    private InventorySystem playerInventory;
    private TextMeshProUGUI actionText;

    private void Start()
    {
        Debug.Log($"{item.name} {item.description} {item.price}");
        item.gameObject = gameObject;
        PlayerController playerController =
            GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerController>();
        playerInventory = playerController.playerInventory;
        actionText = playerController.text;
    }

    private void Update()
    {
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        actionText.text = "";
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // If item in a ranged weapon
            if (item.isRangedWeapon())
            {
                // check if weapon slot is full replace, if not pick up
                if (playerInventory.isRangeWeaponFull())
                {
                    actionText.text = "Press 2 to replace range weapon";

                    if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        gameObject.SetActive(false); // hides object from scene

                        GameObject droppedItem = playerInventory.GetItemAt(1).gameObject;
                        droppedItem.transform.position = transform.position;
                        droppedItem.SetActive(true);

                        playerInventory.SwapItemAt(item, 1);
                    }
                }
                else
                {
                    actionText.text = "Press E to pick up range weapon";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        gameObject.SetActive(false); // hides object from scene
                        playerInventory.SwapItemAt(item, 1);
                    }
                }
            }
            // If item in a melee weapon
            else if (item.isMeleeWeapon())
            {
                // check if weapon slot is full replace, if not pick up
                if (playerInventory.isMeleeWeaponFull())
                {
                    actionText.text = "Press 1 to replace melee weapon";

                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        gameObject.SetActive(false); // hides object from scene

                        GameObject droppedItem = playerInventory.GetItemAt(0).gameObject;
                        droppedItem.transform.position = transform.position;
                        droppedItem.SetActive(true);

                        playerInventory.SwapItemAt(item, 0);
                    }
                }
                else
                {
                    actionText.text = "Press E to pickup melee weapon";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        gameObject.SetActive(false); // hides object from scene
                        playerInventory.SwapItemAt(item, 0);
                    }
                }
            }

            // playerInventory.DisplayInventory();
        }
    }
}