#if USE_CINEMACHINE && USE_INPUTSYSTEM

using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using Yarn.Unity.Addons.SpeechBubbles;
#nullable enable

public class TopDownController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Header("Movement Parameters")]
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Rate at which the character accelerates from standing.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Rate at which the character declerates when stopping.")]
    float walkDeceleration = 70;

    [SerializeField]
    float turnRate = 90f;

    [SerializeField] float gravity = 10f;

    [Header("Movement Actions")]
    [SerializeField]
    InputActionReference? movement;

    [SerializeField]
    InputActionReference? interact;

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

    /// <summary>
    /// The primary dialogue runner is the DialogueRunner for any conversation
    /// that the player is participating in.
    /// </summary>
    /// <remarks>
    /// When this DialogueRunner is active,
    /// the camera switches to the dialogue camera, and the input map is
    /// switched to the Dialogue map.
    /// </remarks>
    [SerializeField] DialogueRunner? primaryDialogueRunner;

    /// <summary>
    /// The character;s 
    /// </summary>
    Vector3 velocity = Vector3.zero;

    [Header("Camera Control")]
    
    /// <summary>
    /// The target group to update when the current Interactable changes.
    /// </summary>
    /// <remarks>
    /// The conversation camera looks at this target group. When the current
    /// Interactable changes, the target group is updated to include the player
    /// and the current Interactable.
    /// </remarks>
    [SerializeField] CinemachineTargetGroup targetGroup;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField, ShowIf(nameof(animator))] string speedParameter = "NormalisedSpeed";

    /// <summary>
    /// The Interactable that we'd interact with if we pressed the
    /// Interact button.
    /// </summary>
    private Interactable? currentInteractable;

    void Awake()
    {
        var dialogueRunner = primaryDialogueRunner;

        if (dialogueRunner == null)
        {
            var allRunners = FindObjectsOfType<DialogueRunner>();
            if (allRunners.Length != 1)
            {
                Debug.LogWarning($"{allRunners.Length} {nameof(DialogueRunner)} objects are present in the scene, and ${nameof(primaryDialogueRunner)} is not set.");
                return;
            }
            else
            {
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

    // Update is called once per frame
    void Update()
    {
        UpdateNearestInteractable();
        UpdateInteraction();
        UpdateMovement();

        if (primaryDialogueRunner != null && primaryDialogueRunner.IsDialogueRunning && this.currentInteractable != null) {
            // We're in conversation, so turn to face our current interactable

            var interactablePosition = currentInteractable.transform.position - this.transform.position;
            interactablePosition.y = 0;

            var targetDirection = Quaternion.LookRotation(interactablePosition, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDirection, turnRate * Time.deltaTime);
        }
    }

    private void UpdateInteraction()
    {
        if (interact != null && currentInteractable != null) {
            var isInteracting = interact.action.WasPerformedThisFrame();

            if (isInteracting) {
                currentInteractable.OnInteracted();
            }
        }
    }

    private void UpdateMovement()
    {
        if (movement != null && controller)
        {
            Vector3 movementInput = movement.action.ReadValue<Vector2>();

            movementInput = new Vector3(
                movementInput.x,
                0,
                movementInput.y
            );

            bool isMoving = Vector3.SqrMagnitude(movementInput) > 0;

            if (isMoving)
            {
                velocity = Vector3.MoveTowards(velocity, speed * movementInput, walkAcceleration * Time.deltaTime);
            }
            else
            {
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, walkDeceleration * Time.deltaTime);
            }

            var translation = velocity * Time.deltaTime;
            controller.Move(translation);

            var down = Vector3.down * gravity * Time.deltaTime;
            controller.Move(down);

            if (isMoving)
            {
                var targetDirection = Quaternion.LookRotation(movementInput, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDirection, turnRate * Time.deltaTime);
            }

        }

        if (animator != null && string.IsNullOrEmpty(speedParameter) == false) {
            var normalizedSpeed = velocity.magnitude / speed;
            animator.SetFloat(speedParameter, normalizedSpeed);
        }
    }

    private void UpdateNearestInteractable()
    {
        Collider[] nearby = new Collider[8];

        int overlapCount = Physics.OverlapSphereNonAlloc(this.transform.position + (Vector3)interactionOffset, interactionRange, nearby);

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
            var previousInteractable = this.currentInteractable;

            if (this.currentInteractable != null)
            {
                this.currentInteractable.OnBecameInactive();
            }

            this.currentInteractable = nearestInteractable;

            if (this.currentInteractable != null)
            {
                this.currentInteractable.OnBecameActive();

                // Update the target group so that it includes the player and
                // this new interactable.
                if (targetGroup != null)
                {
                    targetGroup.m_Targets = new CinemachineTargetGroup.Target[] {
                        new CinemachineTargetGroup.Target{
                            target = this.transform,
                            radius = 1f,
                            weight = 1f,
                        },
                        new CinemachineTargetGroup.Target{
                            target = this.currentInteractable.transform,
                            radius = 1f,
                            weight = 1f,
                        }
                    };
                }
            }

        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position + (Vector3)interactionOffset, interactionRange);
    }

}
#endif