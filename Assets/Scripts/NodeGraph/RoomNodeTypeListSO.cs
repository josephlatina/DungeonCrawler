/*
 * RoomNodeTypeListSO.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Used to create scriptable object assets for Room Node Type List.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// List used in place of an enum to be populated with all different RoomNodeTypeSO
/// </summary>
[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header ROOM NODE TYPE LIST
    [Space(10)]
    [Header("ROOM NODE TYPE LIST")]
    #endregion
    #region Tooltip
    [Tooltip("This list should be populated with all the RoomNodeTypeSO for the game - it is used instead of an enum")]
    #endregion
    public List<RoomNodeTypeSO> list;

    /// <summary>
    /// Gets called as you update a value in a scriptable object. Validates the name of SO.
    /// </summary>
   #region Validation
   #if UNITY_EDITOR
   private void OnValidate() {
    HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
   }
   #endif
   #endregion 
}
