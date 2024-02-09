using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    public GameObject melee;
    public Collider2D meleeTrigger;
    public GameObject ranged;
    public Collider2D rangedTrigger;

    private Transform meleeHolder;
    private Transform rangedHolder;
    private RangedWeaponController rangedController;

    private Camera mainCam;
    public Vector3 mousePos;

    /// <summary>
    /// Called when the script is initialized.
    /// </summary>
    void Start()
    {
        // Get the main camera reference.
        mainCam = Camera.main;
        meleeHolder = transform.Find("Melee").GetComponent<Transform>();
        rangedHolder = transform.Find("Ranged").GetComponent<Transform>();
    }

    public void SetWeapon(GameObject newWeapon, int weaponIndex)
    {
        if (weaponIndex == 0)
        {
            melee = newWeapon;
            melee.transform.parent = meleeHolder;
            melee.transform.position = meleeHolder.position;
            melee.transform.localRotation = Quaternion.Euler(0, 0, -135);
            melee.tag = "Untagged";
            // meleeWeaponController = melee.GetComponent<WeaponItemController>();

            meleeTrigger = melee.GetComponent<Collider2D>();
        }
        else if (weaponIndex == 1)
        {
            ranged = newWeapon;
            ranged.transform.parent = rangedHolder;
            ranged.transform.position = rangedHolder.position;
            ranged.transform.localRotation = Quaternion.Euler(0, 0, -135);
            ranged.tag = "Untagged";
            rangedController = ranged.GetComponent<RangedWeaponController>();
            rangedController.OnPickup();
        }
    }

    public void DropWeapon(int weaponIndex, Vector3 position)
    {
        if (weaponIndex == 0)
        {
            melee.transform.parent = null;
            melee.transform.localRotation = Quaternion.identity;
            melee.tag = "interactableObject";
            melee = null;
        }
        else if (weaponIndex == 1)
        {
            ranged.transform.parent = null;
            ranged.transform.rotation = Quaternion.identity;
            ranged.tag = "interactableObject";
            ranged = null;
        }
    }

    public void FireRanged()
    {
        rangedController.Fire();
    }
}
