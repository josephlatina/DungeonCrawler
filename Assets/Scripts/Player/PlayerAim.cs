/*
 * PlayerAim.cs
 * Author: Josh Coss
 * Created: January 26, 2024
 * Description: Handles player aiming based on mouse position.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player aiming based on mouse position.
/// </summary>
public class PlayerAim : MonoBehaviour
{
    private Camera mainCam;
    public Vector3 mousePos;

    /// <summary>
    /// Called when the script is initialized.
    /// </summary>
    void Start()
    {
        // Get the main camera reference.
        mainCam = Camera.main;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        // Update the mouse position in world coordinates.
        mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Calculate rotation based on mouse position.
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        // Apply the calculated rotation to the object.
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
