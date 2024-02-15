/*
 * HelperUtilities.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Utility helper tool to be used for other classes in terms of validation, etc.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper utility tool
/// </summary>
public static class HelperUtilities
{
    // reference to the main camera
     public static Camera mainCamera;

    /// <summary>
    /// Empty string debug check
    /// </summary>
    /// <param name="thisObject">Object to be validated.</param>
    /// <param name="fieldName">Field within the object.</param>
    /// <param name="stringToCheck">The string value that is to be validated.</param>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck) {

        if (stringToCheck == "") {
            Debug.Log(fieldName +  " is empty and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    /// <summary>
    /// list empty or contains null value check - returns true if there is an error
    /// </summary>
    /// <param name="thisObject">Object to be validated.</param>
    /// <param name="fieldName">Field within the object.</param>
    /// <param name="enumerableObjectToCheck">Any enumerable object (ie. list, array) to iterate through.</param>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck) {
        bool error = false;
        int count = 0;

        // check if the enumerable object itself is null
        if (enumerableObjectToCheck == null) {
            Debug.Log(fieldName + " is null in object " + thisObject.name.ToString());
            return true;
        }

        // iterate through the enumerable objects to check for null values
        foreach (var item in enumerableObjectToCheck) {
            if (item == null) {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
                error = true;
            } else {
                count++;
            }
        }

        // if there are no values to check, return error since it is empty
        if (count == 0) {
            Debug.Log(fieldName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    /// <summary>
    /// Get the mouse world position.
    /// </summary>
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // Clamp mouse position to screen size
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;

    }

    /// <summary>
    /// Get the nearest spawn position to the player
    /// </summary>
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);

        // Loop through room spawn positions
        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            // convert the spawn grid positions to world positions
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;

    }
}
