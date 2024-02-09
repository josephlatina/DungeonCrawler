using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponController : WeaponItemController
{
    public PlayerAim aim;
    private float currentSpeed;
    private Transform playerParent;
    private float counter;
    public float countdown;
    Vector3 direction;

    protected override void Start()
    {
        base.Start();
    }

    public void OnPickup()
    {
        playerParent = transform.parent;
        aim = GetComponentInParent<PlayerAim>();
    }

    public void Fire()
    {
        if (transform.parent)
        {
            counter = countdown;
            currentSpeed = item.GetSpeed();
            transform.parent = null;
            direction = aim.transform.right;
            Debug.DrawRay(transform.position, direction * 100, Color.yellow, 0.25f);
        }
    }

    void Update()
    {
        if (counter > 0)
        {
            transform.position += 10 * currentSpeed * Time.deltaTime * direction;
            AttackCooldown();
        }
    }

    public void ReturnToPlayer()
    {
        transform.parent = playerParent;
        transform.position = playerParent.position;
        transform.localRotation = Quaternion.Euler(0, 0, playerParent.rotation.z - 135);
    }

    void AttackCooldown()
    {
        if (counter > 0)
        {
            counter -= Time.deltaTime;
            if (counter <= 0)
            {
                currentSpeed = 0;
                ReturnToPlayer();
            }
        }
    }
}
