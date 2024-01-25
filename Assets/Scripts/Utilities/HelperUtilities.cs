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

        // iterate through the enumerable object to check for null values
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
}
