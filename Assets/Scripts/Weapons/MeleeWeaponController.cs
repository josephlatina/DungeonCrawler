using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : WeaponItemController
{
    public PlayerAim aim;
    Vector3 direction;
    private Transform playerParent;


    protected override void Start()
    {
        base.Start();
    }

    public void OnPickup()
    {
        playerParent = transform.parent;
        aim = GetComponentInParent<PlayerAim>();
    }

    public void OnDrop()
    {
        playerParent = null;
        transform.parent = null;
        aim = null;
        transform.localRotation = Quaternion.identity;
    }

    public void Attack()
    {

    }

    void Update()
    {

    }
}
