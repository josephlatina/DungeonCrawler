using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private GameObject melee;
    private Transform weaponHold;
    public Collider2D meleeTrigger;
    private GameObject ranged;

    private void Awake() {
        weaponHold = transform;
    }

    public void SetWeapon(GameObject newWeapon)
    {
        
    }

    public GameObject GetMelee()
    {
        return melee;
    }
}
