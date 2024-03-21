/*
 * RangedWeaponController.cs
 * Author: Josh Coss
 * Created: February 12, 2024
 * Description: Controls the behavior of ranged weapons.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponController : WeaponItemController
{
    private PlayerAim aim; // Reference to the player's aim script.
    private float currentSpeed; // Current speed of the projectile.
    private Transform playerParent; // Reference to the parent transform of the player.
    private float counter; // Counter for the attack cooldown.
    public float countdown = 1; // Duration of the attack cooldown.
    Vector3 direction; // Direction of the projectile.

    /// <summary>
    /// Called when the weapon is picked up.
    /// </summary>
    public void OnPickup()
    {
        playerParent = transform.parent;
        aim = GetComponentInParent<PlayerAim>();
    }

    /// <summary>
    /// Called when the weapon is dropped.
    /// </summary>
    public void OnDrop()
    {
        playerParent = null;
        transform.parent = null;
        aim = null;
        transform.localRotation = Quaternion.identity;
        GetComponent<Collider2D>().enabled = true;
    }

    /// <summary>
    /// Fires the ranged weapon.
    /// </summary>
    public void Fire()
    {
        if (transform.parent)
        {
            GetComponent<Collider2D>().enabled = true;
            counter = countdown;
            currentSpeed = item.GetSpeed();
            transform.parent = null;
            direction = aim.transform.right;
            // Debug.DrawRay(transform.position, direction * 100, Color.yellow, 0.25f);
        }
    }

    protected override void Update()
    {
        if (counter > 0)
        {
            transform.position += 10 * currentSpeed * Time.deltaTime * direction;
            AttackCooldown();
        }

        base.Update();
    }

    /// <summary>
    /// Returns the projectile to the player after a certain time.
    /// </summary>
    public void ReturnToPlayer()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = false;
        transform.parent = playerParent;
        transform.position = playerParent.position;
        transform.localRotation = Quaternion.Euler(0, 0, playerParent.rotation.z - 135);
    }

    /// <summary>
    /// Manages the cooldown for the ranged weapon attack.
    /// </summary>
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


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            IEffectable effectable = col.gameObject.GetComponentInParent<IEffectable>();
            if (effectable != null)
            {
                effectable.ApplyEffect(statusEffect);
            }
            col.gameObject.GetComponentInParent<EnemyHealth>().ChangeHealth(CalculateDamageDone());
            if (item.GetKnockback() > 0)
            {
                Vector2 dir = col.transform.position - transform.position;
                col.gameObject.GetComponentInParent<EnemyController>().Knockback(item.GetKnockback() * dir.normalized, 0.75f);
            }
            GetComponent<Collider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            col.GetComponent<EnemyController>().anim.SetTrigger("isHurt");
        }
    }
}
