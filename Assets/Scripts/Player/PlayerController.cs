/*
 * PlayerController.cs
 * Author: Josh Coss, Jehdi Aizon
 * Created: January 16, 2024
 * Description: Handles player input, movement, and interactions.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using Yarn.Unity.Addons.SpeechBubbles;
using Random = UnityEngine.Random;

/// <summary>
/// Handles player input, movement, and interactions.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    [HideInInspector] public Rigidbody2D rb;
    private PlayerStats player;

    [HideInInspector] public Vector2 moveVal;
    public float rollSpeed;
    public float rollLength = 0.5f, rollCooldown = 1f;

    public PlayerStateMachine PlayerStateMachine => playerStateMachine;
    [HideInInspector] public Collider2D meleeTrigger;
    private Collider2D interactTrigger;
    private Transform interactableObject;
    [HideInInspector] public Vector3 mousePos;
    private Transform interactNPC;

    // Set this to the bubble dialogue view you want to control
    public BubbleDialogueView dialogueView;
    public DialogueRunner dialogueRunner;

    private enum PlayerStatus
    {
        Normal,
        Stun,
        Immobilized,
        Poison
    }

    private PlayerStatus status;
    [HideInInspector] public bool isMeleeAttacking, isRangedAttacking, isHealing, isRolling;

    [Header("Inventory System"), Space(5)]
    // Player Inventory System reference to Scriptable Object
    public InventorySystem playerInventory;

    public TextMeshProUGUI text;

    /// <summary>
    /// Called once when script is initialized.
    /// </summary>
    private void Awake()
    {
        // Initialize state machine on player
        playerStateMachine = new PlayerStateMachine(this);
        // Get object's Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        // Get player object's stats script
        player = GetComponent<PlayerStats>();
        // Set initial status to normal
        status = PlayerStatus.Normal;
        // Get player object's melee trigger
        meleeTrigger = transform.Find("AimPivot/MeleeCollider").GetComponent<Collider2D>();
        // Get player object's interaction trigger
        interactTrigger = transform.Find("InteractTrigger").GetComponent<Collider2D>();
    }

    /// <summary>
    /// Called once after Awake.
    /// </summary>
    private void Start()
    {
        // Initialize the state machine with the idle state
        playerStateMachine.Initialize(playerStateMachine.idleState);
        // Initialize inventory
        playerInventory.InitializeInventory();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    private void Update()
    {
        // Update the state machine logic
        playerStateMachine.Update();
    }

    /// <summary>
    /// Called at fixed time intervals, making it suitable for physics-related calculations
    /// to ensure consistent behavior across varying frame rates.
    /// </summary>
    private void FixedUpdate()
    {
        playerStateMachine.FixedUpdate();
    }

    /// <summary>
    /// Called when the application is quitting.
    /// </summary>
    private void OnApplicationQuit()
    {
        playerInventory.Reset();
    }

    /// <summary>
    /// Listens for the player input and updates moveVal to the current value of movement input.
    /// </summary>
    /// <param name="value">Movement vector sent from Player Input component.</param>
    void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }

    /// <summary>
    /// Moves the player in all 8 directions.
    /// </summary>
    public void Move()
    {
        rb.velocity = moveVal * player.CurrentMoveSpeed;
    }

    /// <summary>
    /// Listens for the roll input.
    /// </summary>
    /// <param name="value">Input value for the roll.</param>
    void OnRoll(InputValue value)
    {
        if (!isHealing && !isRangedAttacking && !isMeleeAttacking && moveVal.x != 0 || moveVal.y != 0)
        {
            isRolling = value.isPressed;
        }
    }

    /// <summary>
    /// Listens for the melee attack input.
    /// </summary>
    /// <param name="value">Input value for the melee attack.</param>
    void OnMeleeAttack(InputValue value)
    {
        if (!isHealing && !isRangedAttacking && !isRolling)
        {
            isMeleeAttacking = value.isPressed;
        }
    }

    /// <summary>
    /// Listens for the ranged attack input.
    /// </summary>
    /// <param name="value">Input value for the ranged attack.</param>
    void OnRangedAttack(InputValue value)
    {
        if (!isHealing && !isMeleeAttacking && !isRolling)
        {
            isRangedAttacking = value.isPressed;
        }
    }

    /// <summary>
    /// Listens for the interact input.
    /// </summary>
    /// <param name="value">Input value for the interact action.</param>
    void OnInteract(InputValue value)
    {
        if (interactableObject)
        {
            WeaponItemController weapon =
                interactableObject.gameObject.GetComponent<WeaponItemController>();
            ConsumableItemController consumable =
                interactableObject.gameObject.GetComponent<ConsumableItemController>();
            // if object is a weapon
            if (weapon != null)
            {
                int weaponIndex = weapon.item.isRangedWeapon() ? 1 : 0;
                InventoryItem dropItem = playerInventory.GetItemAt(weaponIndex);

                weapon.gameObject.SetActive(false); // hides object from scene

                // check if range weapon slot is full replace, if not pick up
                if (playerInventory.isRangeWeaponFull() || playerInventory.isMeleeWeaponFull())
                {
                    if (dropItem != null)
                    {
                        dropItem.DropItemAt(transform.position);
                    }
                }

                playerInventory.SwapItemAt(weapon.item, weaponIndex);
            }
            // if object is a consumable
            else if (consumable != null)
            {
                InventoryItem dropItem = playerInventory.GetItemAt(2);

                consumable.gameObject.SetActive(false); // hides object from scene

                // check if consumable slots are full replace, if not pick up
                if (playerInventory.isConsumableFull())
                {
                    dropItem.DropItemAt(transform.position);
                }

                playerInventory.SwapItemAt(consumable.item, 2);
            }
        }
        else if (interactNPC)
        {
            if (!dialogueView.IsShowingBubble)
            {
                string node = interactNPC.GetComponent<NpcController>().GetNode();
                dialogueRunner.StartDialogue(node);
            }
        }
    }

    void OnAdvanceDialogue(InputValue value)
    {
        // If we're not showing bubbles, do nothing
        if (dialogueView.IsShowingBubble == false)
        {
            return;
        }

        switch (dialogueView.CurrentContentType)
        {
            case BubbleDialogueView.ContentType.Options:
                // If we're showing options, select the current option
                dialogueView.SelectOption();
                break;
            case BubbleDialogueView.ContentType.Line:
                // If we're showing a line, advance to the next line
                dialogueView.UserRequestedViewAdvancement();
                break;
        }
    }

    /// <summary>
    /// Listens for the heal input.
    /// </summary>
    /// <param name="value">Input value for the heal action.</param>
    void OnHeal(InputValue value)
    {
        if (!isHealing)
        {
            isHealing = value.isPressed;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "interactableObject" && interactableObject == null)
        {
            interactableObject = other.transform;
        }
        else if (other.CompareTag("NPC") && interactNPC == null)
        {
            interactNPC = other.transform;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("interactableObject") && interactableObject == null)
        {
            interactableObject = other.transform;
        }
        else if (other.CompareTag("NPC") && interactNPC == null)
        {
            interactNPC = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "interactableObject" && interactableObject != null)
        {
            interactableObject = null;
        }
        else if (other.CompareTag("NPC") && interactNPC != null)
        {
            interactNPC = null;
        }
    }
}