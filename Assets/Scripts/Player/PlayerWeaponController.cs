/*
 * PlayerWeaponController.cs
 * Author: Josh Coss
 * Created: February 12, 2024
 * Description: Controls the player's weapons, including melee and ranged attacks.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    public GameObject melee; // Reference to the melee weapon GameObject.
    public Collider2D meleeTrigger; // Collider for the melee weapon trigger.
    public GameObject ranged; // Reference to the ranged weapon GameObject.
    public Collider2D rangedTrigger; // Collider for the ranged weapon trigger.

    private Transform meleeHolder; // Transform to hold the melee weapon.
    private MeleeWeaponController meleeController; // Reference to the MeleeWeaponController script.
    private Transform rangedHolder; // Transform to hold the ranged weapon.
    private RangedWeaponController rangedController; // Reference to the RangedWeaponController script.

    private Camera mainCam; // Reference to the main camera.
    public Vector3 mousePos; // Mouse position in the world.

    /// <summary>
    /// Called when the script is initialized.
    /// </summary>
    void Start()
    {
        // Get the main camera reference.
        mainCam = Camera.main;
        // Find the melee holder object.
        meleeHolder = transform.Find("Pivot/Melee").GetComponent<Transform>();
        // Find the ranged holder object.
        rangedHolder = transform.Find("Ranged").GetComponent<Transform>();
    }

    /// <summary>
    /// Set the weapon based on the provided weapon index.
    /// </summary>
    /// <param name="newWeapon">The new weapon GameObject.</param>
    /// <param name="weaponIndex">The index indicating the type of weapon (0 for melee, 1 for ranged).</param>
    public void SetWeapon(GameObject newWeapon, int weaponIndex)
    {
        if (weaponIndex == 0)
        {
            // Set up the melee weapon.
            melee = newWeapon;
            melee.transform.parent = meleeHolder;
            melee.transform.position = meleeHolder.position;
            melee.transform.localRotation = Quaternion.Euler(0, 0, -135);
            melee.tag = "Untagged";
            meleeController = melee.GetComponent<MeleeWeaponController>();
            meleeController.OnPickup();
        }
        else if (weaponIndex == 1)
        {
            // Set up the ranged weapon.
            ranged = newWeapon;
            ranged.transform.parent = rangedHolder;
            ranged.transform.position = rangedHolder.position;
            ranged.transform.localRotation = Quaternion.Euler(0, 0, -135);
            ranged.tag = "Untagged";
            rangedController = ranged.GetComponent<RangedWeaponController>();
            rangedController.OnPickup();
        }
    }

    /// <summary>
    /// Drop the weapon based on the provided weapon index and position.
    /// </summary>
    /// <param name="weaponIndex">The index indicating the type of weapon (0 for melee, 1 for ranged).</param>
    /// <param name="position">The position where the weapon should be dropped.</param>
    public void DropWeapon(int weaponIndex, Vector3 position)
    {
        if (weaponIndex == 0)
        {
            // Drop the melee weapon.
            melee.transform.parent = null;
            melee.transform.localRotation = Quaternion.identity;
            melee.tag = "interactableObject";
            meleeController.OnDrop();
            meleeController = null;
            melee = null;
        }
        else if (weaponIndex == 1)
        {
            // Drop the ranged weapon.
            ranged.transform.parent = null;
            ranged.transform.rotation = Quaternion.identity;
            ranged.tag = "interactableObject";
            rangedController.OnDrop();
            rangedController = null;
            ranged = null;
        }
    }

    /// <summary>
    /// Fire the ranged weapon.
    /// </summary>
    public void FireRanged()
    {
        rangedController.Fire();
    }

    /// <summary>
    /// Perform a melee attack.
    /// </summary>
    public void MeleeAttack()
    {
        meleeController.Attack();
    }
}
