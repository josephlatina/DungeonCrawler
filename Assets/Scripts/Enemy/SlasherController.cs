/*
 * SlasherController.cs
 * Author: Josh Coss
 * Created: January 16, 2024
 * Description: Handles Slasher-specific logic.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the behavior specific to the Slasher enemy type.
/// </summary>
public class SlasherController : EnemyController
{
    // FixedUpdate is used for physics-related updates
    protected override void FixedUpdate()
    {
        // Call the Move method for the slasher's movement
        Move();
        base.FixedUpdate();
    }

    /// <summary>
    /// Handles the horizontal movement of the Slasher.
    /// </summary>
    private void Move()
    {
        if (knockbackDuration > 0)
        {
            rb.AddForce(knockbackVelocity * Time.deltaTime, ForceMode2D.Impulse);
            rb.velocity = knockbackVelocity;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
        // Add force to the Rigidbody for horizontal movement (USED FOR DEBUGGING ONLY)
        // Note: Using Time.deltaTime to make the movement frame-rate independent
        // rb.AddForce(new Vector2(movementSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);
    }

    public float DoDamage()
    {
        return strength;
    }
}
