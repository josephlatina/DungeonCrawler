using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode up = KeyCode.W;
    [SerializeField] private KeyCode down = KeyCode.S;
    [SerializeField] private KeyCode left = KeyCode.A;
    [SerializeField] private KeyCode right = KeyCode.D;

    public Vector2 InputVector => inputVector;

    private Vector2 inputVector;
    private float xInput;
    private float yInput;

    public void HandleInput()
    {
        xInput = 0;
        yInput = 0;

        if (Input.GetKey(up))
        {
            yInput++;
        }

        if (Input.GetKey(down))
        {
            yInput--;
        }

        if (Input.GetKey(left))
        {
            xInput--;
        }
        if (Input.GetKey(right))
        {
            xInput++;
        }

        inputVector = new Vector3(xInput, yInput);
    }

    private void Update()
    {
        HandleInput();
    }
}
