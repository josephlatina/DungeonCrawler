/*
 * PlayerHealState.cs
 * Author: Josh Coss
 * Created: January 26 2024
 * Description: Handles state transitions to and from the Heal state, as well as update logic for the state.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Heal state logic for the player.
/// </summary>
public class PlayerHealState : IState
{
    private PlayerController player;
    private PlayerHealth health;
    private ConsumableItem potion;

    private float healCounter;

    // Constructor to initialize the state with the associated player controller.
    public PlayerHealState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// Runs when first entering the heal state.
    /// </summary>
    public void Enter()
    {
        // Change player's color to green when entering the heal state.
        player.anim.transform.Find("CharacterSprite").GetComponent<SpriteRenderer>().color = Color.green;
        // Get the player's PlayerHealth component
        health = player.GetComponent<PlayerHealth>();
        potion = player.playerInventory.GetConsumable();
        if (potion)
        {
            Heal();
        }
        Debug.Log("Player is healing");
    }

    /// <summary>
    /// Runs when exiting the heal state.
    /// </summary>
    public void Exit()
    {
        // Reset player's color to white when exiting the heal state.
        player.anim.transform.Find("CharacterSprite").GetComponent<SpriteRenderer>().color = Color.white;
        Debug.Log("Player is finished healing");

        // Reset the heal counter.
        healCounter = 0;
    }

    /// <summary>
    /// Runs at fixed time intervals, making it suitable for physics-related calculations.
    /// </summary>
    public void FixedUpdate()
    {
        // Move the player.
        player.Move();
    }

    /// <summary>
    /// Per-frame logic for the heal state - Include condition to transition to a new state.
    /// </summary>
    public void Update()
    {
        // Perform heal cooldown logic.
        HealCooldown();

        // Check conditions to transition to a different state.
        if (!player.isHealing)
        {
            if (player.moveVal.x > 0.1f || player.moveVal.y > 0.1f)
            {
                // Transition to move state if the player is moving.
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
            }
            else
            {
                // Transition to idle state if the player is not moving.
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
            }
        }
    }

    void Heal()
    {
        health.ChangeHealth(potion.healthRestore);
        player.playerInventory.EmptyConsumable();
    }

    /// <summary>
    /// Manages the cooldown for healing.
    /// </summary>
    void HealCooldown()
    {
        if (healCounter <= 0)
        {
            // Set the heal counter when it reaches zero.
            healCounter = 0.5f; // TODO: This will be replaced by the length of the heal animation.
        }
        if (healCounter > 0)
        {
            // Decrease the heal counter and check for transition conditions.
            healCounter -= Time.deltaTime;
            if (healCounter <= 0)
            {
                // End the healing process.
                player.isHealing = false;
            }
        }
    }
}
