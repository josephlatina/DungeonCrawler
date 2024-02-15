using System;
using System.Linq;
using UnityEngine;

#if USE_INPUTSYSTEM
using UnityEngine.InputSystem;
using Yarn.Unity;

#nullable enable

public class SidescrollerController : MonoBehaviour
{
    public enum SpriteDirection { Left, Right };
    [Header("Graphics and Animation")]
    [SerializeField] SpriteRenderer? sprite;

    [SerializeField] SpriteDirection spriteFacingDirection = SpriteDirection.Right;

    [SerializeField] Animator? animator;

    [Header("Movement Parameters")]
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;

    [SerializeField, Tooltip("Gravity speed")]
    float gravity = 10f;

    [Header("Movement Actions")]
    [SerializeField]
    InputActionReference? horizontalMovement;

    [SerializeField]
    InputActionReference? interact;

    private BoxCollider2D boxCollider;
    private Vector2 velocity;

    /// <summary>
    /// Set to true when the character intersects a collider beneath
    /// them in the previous frame.
    /// </summary>
    private bool grounded;

    [Header("Action Maps")]
    [SerializeField]
    private string dialogueInputActionMap = "Dialogue";
    [SerializeField]
    private string movementInputActionMap = "Player";

    [Header("Interaction")]
    [SerializeField]
    private float interactionRange = 1f;

    [SerializeField]
    private Vector2 interactionOffset = Vector2.zero;

    [SerializeField] DialogueRunner? primaryDialogueRunner;

    /// <summary>
    /// The SideScrollerInteractable that we'd interact with if we pressed the
    /// Interact button.
    /// </summary>
    private Interactable? currentInteractable;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        var dialogueRunner = primaryDialogueRunner;

        if (dialogueRunner == null)
        {
            var allRunners = FindObjectsOfType<DialogueRunner>();
            if (allRunners.Length != 1) {
                Debug.LogWarning($"{allRunners.Length} {nameof(DialogueRunner)} objects are present in the scene, and ${nameof(primaryDialogueRunner)} is not set.");
                return;
            } else {
                dialogueRunner = allRunners[0];
            }
        }

        var playerInput = GetComponent<PlayerInput>();

        dialogueRunner.onDialogueStart.AddListener(() =>
        {
            playerInput.SwitchCurrentActionMap(dialogueInputActionMap);
        });
        dialogueRunner.onDialogueComplete.AddListener(() =>
        {
            playerInput.SwitchCurrentActionMap(movementInputActionMap);
        });

        playerInput.SwitchCurrentActionMap(movementInputActionMap);
    }

    struct MovementInput
    {
        public float horizontal;
        public bool startedJumping;
        public bool startedInteracting;
    }

    MovementInput GetMovementInput()
    {
        var result = new MovementInput();

        if (horizontalMovement != null)
        {
            result.horizontal = horizontalMovement.action.ReadValue<Vector2>().x;
        }
        else
        {
            // Fall back to keyboard input
            result.horizontal += Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            result.horizontal -= Keyboard.current.leftArrowKey.isPressed ? 1 : 0;
        }

        if (interact != null) {
            result.startedInteracting = interact.action.WasPerformedThisFrame();
        } else {
            // Fall back to keyboard input
            result.startedInteracting = Keyboard.current.eKey.wasPressedThisFrame;
        }

        return result;
    }

    private void Update()
    {
        UpdateNearestInteractable();

        var input = GetMovementInput();

        ApplyMovement(input);
        ApplyInteraction(input);
    }

    private void ApplyInteraction(MovementInput input)
    {
        if (this.currentInteractable == null)
        {
            return;
        }

        if (input.startedInteracting)
        {
            this.currentInteractable.OnInteracted();
        }
    }

    private void UpdateNearestInteractable()
    {
        Collider2D[] nearby = new Collider2D[8];

        var filter = new ContactFilter2D();
        filter.useTriggers = true;

        int overlapCount = Physics2D.OverlapCircle(this.transform.position + (Vector3) interactionOffset, interactionRange, filter, nearby);

        Interactable? nearestInteractable = null;

        for (int i = 0; i < overlapCount; i++)
        {
            var other = nearby[i];

            if (other.gameObject.TryGetComponent<Interactable>(out var interactable))
            {
                nearestInteractable = interactable;
            }
        }

        if (nearestInteractable != this.currentInteractable)
        {
            if (this.currentInteractable != null)
            {
                this.currentInteractable.OnBecameInactive();
            }

            this.currentInteractable = nearestInteractable;

            if (this.currentInteractable != null)
            {
                this.currentInteractable.OnBecameActive();
            }
        }
    }

    private void ApplyMovement(MovementInput input)
    {
        if (grounded)
        {
            velocity.y = 0;

            if (input.startedJumping)
            {
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(-gravity));
            }
        }

        float acceleration = grounded ? walkAcceleration : airAcceleration;
        float deceleration = grounded ? groundDeceleration : 0;

        if (input.horizontal != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * input.horizontal, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        velocity.y += -gravity * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);

        grounded = false;

        Physics2D.SyncTransforms();

        // Retrieve all colliders we have intersected after velocity has been applied.
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit == boxCollider)
                continue;

            if (hit.isTrigger)
            {
                continue;
            }

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object beneath us, set grounded to true. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                {
                    grounded = true;
                }
            }
        }

        if (grounded)
        {
            if (Mathf.Abs(input.horizontal) > 0.01f)
            {
                if (sprite != null)
                {
                    // We're on the ground and moving horizontally; update our facing
                    // direction
                    switch (spriteFacingDirection)
                    {
                        case SpriteDirection.Left:
                            sprite.flipX = velocity.x > 0;
                            break;
                        case SpriteDirection.Right:
                            sprite.flipX = velocity.x < 0;
                            break;
                    }
                }
            }
        }
        if (animator != null)
        {
            if (Mathf.Abs(input.horizontal) > 0.01f)
            {
                animator.Play("Walk");
            }
            else
            {
                animator.Play("Idle");
            }
        }

    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position + (Vector3)interactionOffset, interactionRange);
    }
}
#endif