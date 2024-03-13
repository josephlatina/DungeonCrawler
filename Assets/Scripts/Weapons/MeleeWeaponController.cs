/*
 * MeleeWeaponController.cs
 * Author: Josh Coss
 * Created: February 12, 2024
 * Description: Controls the behavior of melee weapons.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : WeaponItemController
{

    public PlayerAim aim; // Reference to the player's aim script.
    private Animator anim; // Reference to the animator component.
    Vector3 direction; // Direction of the weapon swing.
    private Transform playerParent; // Reference to the parent transform of the player.

    /// <summary>
    /// Called when the weapon is picked up.
    /// </summary>
    public void OnPickup()
    {
        playerParent = transform.parent;
        // Get the PlayerAim component from the parent of the parent (PlayerController).
        aim = playerParent.GetComponentInParent<PlayerAim>();
        // Get the Animator component from the parent of the parent of the parent (PlayerController).
        anim = aim.transform.GetComponentInParent<PlayerWeaponController>().GetComponentInParent<PlayerController>().anim;
        GetComponent<Collider2D>().enabled = false;
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
    /// Initiates a melee attack.
    /// </summary>
    public void Attack()
    {
        if (transform.parent)
        {
            // GetComponent<Collider2D>().enabled = true;
            anim.SetTrigger("attack");
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            IEffectable effectable = col.GetComponent<IEffectable>();
            if (effectable != null)
            {
                effectable.ApplyEffect(statusEffect);
            }
            col.gameObject.GetComponentInParent<EnemyHealth>().ChangeHealth(CalculateDamageDone());
            if (item.GetKnockback() > 0)
            {
                Vector2 dir = col.transform.position - transform.position;
                Debug.Log(dir.normalized * item.GetKnockback());
                col.gameObject.GetComponentInParent<EnemyController>().Knockback(item.GetKnockback() * dir.normalized, 0.75f);
            }
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
