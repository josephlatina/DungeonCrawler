using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    private GameObject melee;
    public Collider2D meleeTrigger;
    private GameObject ranged;

    private Camera mainCam;
    public Vector3 mousePos;

    /// <summary>
    /// Called when the script is initialized.
    /// </summary>
    void Start()
    {
        // Get the main camera reference.
        mainCam = Camera.main;
    }

    public void SetWeapon(GameObject newWeapon, int weaponIndex)
    {
        if (weaponIndex == 0)
        {
            melee = newWeapon;
            melee.transform.parent = transform;
            melee.transform.position = transform.position;
            melee.transform.localRotation = Quaternion.Euler(0, 0, -135);
            melee.tag = "Untagged";

            meleeTrigger = melee.GetComponent<Collider2D>();
        }
        else if (weaponIndex == 1)
        {
            ranged = newWeapon;
            ranged.transform.parent = transform;
            ranged.transform.position = transform.position;
            ranged.tag = "Untagged";
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

    void Update()
    {
        // if (melee)
        // {
        //     melee.transform.rotation = transform.parent.rotation;
        // }
    }

    public GameObject GetMelee()
    {
        return melee;
    }
}
