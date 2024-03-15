/*
 * MovementToPosition.cs
 * Author: Joseph Latina
 * Created: March 11, 2024
 * Description: Used to monitor the movement to position event
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private Rigidbody2D childRigidbody2D;
    private SpriteRenderer spriteRenderer;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        // Load Components
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        // Get the child object's Transform component
        Transform childTransform = transform.GetChild(0); // Adjust index as needed

        // Get the Rigidbody2D component of the child object
        childRigidbody2D = childTransform.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Subscribe to movement to position event
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        // Unsubscribe from movement to position event
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    // On movement event
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        MoveRigidBody(movementToPositionArgs.movePosition, movementToPositionArgs.currentPosition, movementToPositionArgs.moveSpeed);
    }

    /// <summary>
    /// Move the rigidbody component
    /// </summary>
    private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);
        if (unitVector.x > 0) {
            spriteRenderer.flipX = false;
        } 
        else if (unitVector.x < 0) {
            spriteRenderer.flipX = true;
        }
        rigidBody2D.velocity = unitVector * moveSpeed;
        rigidBody2D.MovePosition(rigidBody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));

    }
}
