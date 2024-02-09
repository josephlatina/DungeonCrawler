/*
 * ConsumableItemController.cs
 * Author: Jehdi Aizon
 * Created: January 22, 2024
 * Description: This is the script that attaches to the weapon prefab.
 * It houses the scriptable object of a Weapon Item.
 * Determines behaviour of weapon items.
 */

using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponItemController : MonoBehaviour
{
    // Reference to the ScriptableObject of the Weapon Item type
    public WeaponItem item;
    public InventorySystem playerInventory;
    public TextMeshProUGUI actionText;
    public SpriteRenderer sprite;


    protected virtual void Start()
    {
        item.gameObject = gameObject; // reference current game object to scriptable object
        PlayerController playerController =
            GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerController>();
        playerInventory = playerController.playerInventory;
        actionText = playerController.text;
        sprite = gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        if (item.itemSprite)
        {
            sprite.sprite = item.itemSprite;
            sprite.color = Color.white;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        actionText.text = "";
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("interactTrigger"))
        {
            // If item in a ranged weapon
            if (item.isRangedWeapon() && gameObject.tag == "interactableObject")
            {
                // check if weapon slot is full replace, if not pick up
                if (playerInventory.isRangeWeaponFull())
                {
                    actionText.text = $"Press E to replace range weapon with {item.itemName}";
                }
                else
                {
                    actionText.text = $"Press E to pick up range weapon {item.itemName}";
                }
            }
            // If item in a melee weapon
            else if (item.isMeleeWeapon() && gameObject.tag == "interactableObject")
            {
                // check if weapon slot is full replace, if not pick up
                if (playerInventory.isMeleeWeaponFull())
                {
                    actionText.text = $"Press E to replace melee weapon with {item.itemName}";
                }
                else
                {
                    actionText.text = $"Press E to pick up melee weapon {item.itemName}";
                }
            }
            else
            {
                actionText.text = "";
            }

            // playerInventory.DisplayInventory();
        }
    }
}