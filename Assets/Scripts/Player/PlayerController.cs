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
    public Animator anim;

    [HideInInspector] public Vector2 moveVal;
    public float rollSpeed;
    public float rollLength = 0.5f, rollCooldown = 1f;

    public PlayerStateMachine PlayerStateMachine => playerStateMachine;
    private Collider2D interactTrigger;
    private Transform interactableObject;
    public PlayerWeaponController weaponController;
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
        // meleeTrigger = transform.Find("AimPivot/MeleeCollider").GetComponent<Collider2D>();
        // Get player object's interaction trigger
        interactTrigger = transform.Find("InteractTrigger").GetComponent<Collider2D>();
        weaponController = GetComponentInChildren<PlayerWeaponController>();
        anim = GetComponentInChildren<Animator>();
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
        if (moveVal.x > 0)
        {
            anim.transform.Find("CharacterSprite").GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveVal.x < 0)
        {
            anim.transform.Find("CharacterSprite").GetComponent<SpriteRenderer>().flipX = true;
        }
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
            ConsumableItemController consumable =
                interactableObject.gameObject.GetComponent<ConsumableItemController>();
            WeaponItemController weapon =
                interactableObject.gameObject.GetComponent<WeaponItemController>();

            if (consumable != null)
            {
                HandleConsumable(consumable);
            }
            // if object is a weapon
            else if (weapon != null)
            {
                int weaponIndex = weapon.item.isRangedWeapon() ? 1 : 0;
                InventoryItem dropItem = playerInventory.GetItemAt(weaponIndex);

                // weapon.gameObject.SetActive(false); // hides object from scene


                // check if range weapon slot is full replace, if not pick up
                if (playerInventory.isRangeWeaponFull() || playerInventory.isMeleeWeaponFull())
                {
                    if (dropItem != null)
                    {
                        dropItem.DropItemAt(transform.position);
                        weaponController.DropWeapon(weaponIndex, weaponController.transform.position);
                    }
                }

                playerInventory.SwapItemAt(weapon.item, weaponIndex);
                weaponController.SetWeapon(weapon.gameObject, weaponIndex);

                interactableObject = null;
            }
        }
        else if (interactNPC)
        {
            if (!dialogueView.IsShowingBubble)
            {
                StartNPCDialogue();
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
    /// Listener for when dialogue for nodeName is complete
    /// </summary>
    /// <param name="nodeName">string name of node</param>
    void HandleNodeComplete(string nodeName)
    {
        GetComponent<PlayerInput>().enabled = true;
        dialogueRunner.onNodeComplete.RemoveListener(HandleNodeComplete);
    }

    /// <summary>
    /// Handles how consumables should be used
    /// </summary>
    /// <param name="consumable">ConsumableItemController reference</param>
    void HandleConsumable(ConsumableItemController consumable)
    {
        consumable.gameObject.SetActive(false); // hides object from scene

        if (consumable.item.itemName == "Pill")
        {
            // listener for when node is complete
            dialogueRunner.onNodeComplete.AddListener(HandleNodeComplete);

            // pause game when pill is picked up
            GetComponent<PlayerInput>().enabled = false;
            dialogueRunner.StartDialogue("PillUpgrade");
        }
        else
        {
            InventoryItem dropItem = playerInventory.GetItemAt(2);

            // check if consumable slots are full replace, if not pick up
            if (playerInventory.isConsumableFull())
            {
                dropItem.DropItemAt(transform.position);
            }

            playerInventory.SwapItemAt(consumable.item, 2);
        }
    }

    /// <summary>
    /// Update player stats. Adds/subtract the number given to the current value of player stat.
    /// </summary>
    /// <param name="health"></param>
    /// <param name="moveSpeed"></param>
    /// <param name="attackSpeed"></param>
    /// <param name="strength"></param>
    /// <param name="defence"></param>
    /// <param name="incomingDamage"></param>
    void UpdatePlayerStats(float health = 0f, float moveSpeed = 0f, float attackSpeed = 0f,
        float strength = 0f,
        int defence = 0, float incomingDamage = 0f)
    {
        player.CurrentHealth += health;
        player.CurrentMoveSpeed += moveSpeed;
        player.CurrentAttackSpeed += attackSpeed;
        player.CurrentStrength += strength;
        player.CurrentDefence += defence;
        player.CurrentIncomingDamage += incomingDamage;
    }

    /// <summary>
    /// Function used in yarn script to
    /// update stat when upgrade item is picked up
    /// </summary>
    /// <param name="attackSpeed"></param>
    /// <param name="strength"></param>
    /// <param name="defence"></param>
    [YarnCommand("stat_upgrade")]
    public void StatUpgrade(float attackSpeed, float strength, int defence)
    {
        UpdatePlayerStats(attackSpeed: attackSpeed, strength: strength, defence: defence);
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
            StartNPCDialogue();
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
            StartNPCDialogue();
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

    void StartNPCDialogue()
    {
        if (!dialogueView.IsShowingBubble)
        {
            NpcController npcController = interactNPC.GetComponent<NpcController>();
            string node = npcController.randomizeDialogue ? npcController.GetRandomNode() : npcController.GetNode();
            dialogueRunner.StartDialogue(node);
        }
    }
}