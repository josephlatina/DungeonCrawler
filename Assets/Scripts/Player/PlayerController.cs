/*
 * PlayerController.cs
 * Author: Josh Coss, Jehdi Aizon
 * Created: January 16, 2024
 * Description: Handles player input, movement, and interactions.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
            // change color of interactable object
            Color newColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            interactableObject.GetComponent<SpriteRenderer>().color = newColor;
            Debug.Log(interactableObject.name);

            // if object is a weapon
            if (interactableObject.gameObject.GetComponent<WeaponItemController>())
            {
                WeaponItemController weapon = interactableObject.gameObject.GetComponent<WeaponItemController>();

                weapon.gameObject.SetActive(false); // hides object from scene

                // check if range weapon slot is full replace, if not pick up
                if (playerInventory.isRangeWeaponFull() || playerInventory.isMeleeWeaponFull())
                {
                    weapon.DropItemAt(transform.position);
                }

                // weapon is ranged
                if (weapon.item.isRangedWeapon())
                {
                    playerInventory.SwapItemAt(weapon.item, 1);
                }
                // weapon in melee
                else if (weapon.item.isMeleeWeapon())
                {
                    playerInventory.SwapItemAt(weapon.item, 0);
                }
            }
            // if object is a consumable
            else if (interactableObject.gameObject.GetComponent<ConsumableItemController>())
            {
                ConsumableItemController consumable =
                    interactableObject.gameObject.GetComponent<ConsumableItemController>();

                consumable.gameObject.SetActive(false); // hides object from scene

                // check if consumable slots are full replace, if not pick up
                if (playerInventory.isConsumableFull())
                {
                    consumable.DropItemAt(transform.position);
                }

                playerInventory.SwapItemAt(consumable.item, 2);
            }
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
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "interactableObject" && interactableObject != null)
        {
            interactableObject = null;
        }
    }
}