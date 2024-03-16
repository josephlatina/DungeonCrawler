/*
 * MovementToPositionEvent.cs
 * Author: Joseph Latina
 * Created: March 11, 2024
 * Description: Script for containing the movement event that can be subscribed by other classes
 */


using System;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementToPositionEvent : MonoBehaviour
{
    // movement event - delegate event variable
    public event Action<MovementToPositionEvent, MovementToPositionArgs> OnMovementToPosition;

    // event is triggered by invoking the delegate event variable and passing in the required parameters - movePosition, currentPosition, movespeed, movedirection, isrolling
    public void CallMovementToPositionEvent(Vector3 movePosition, Vector3 currentPosition, float moveSpeed, Vector2 moveDirection)
    {
        OnMovementToPosition?.Invoke(this, new MovementToPositionArgs() { movePosition = movePosition, currentPosition = currentPosition, moveSpeed = moveSpeed, moveDirection = moveDirection});
    }
}

public class MovementToPositionArgs : EventArgs
{
    public Vector3 movePosition;
    public Vector3 currentPosition;
    public float moveSpeed;
    public Vector2 moveDirection;
}
