/*
 * RoomNodeTypeSO.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Used to create scriptable object assets for Room Node Type.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to define what type of room a Rome Node object is.
/// </summary>
[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    // name of this room node type
   public string roomNodeTypeName;

    // determines whether this room node type should appear in the Node Graph Editor as some will not be shown
   #region Header
   [Header("Only flag the RoomNodeTypes that should be visible in the editor")]
   #endregion Header
   public bool displayInNodeGraphEditor = true;
   // boolean value to identify if this is a corridor type
   #region Header
   [Header("One Type Should Be A Corridor")]
   #endregion Header
   public bool isCorridor;
   // boolean value that the algorithm would use to determine the direction
   #region Header
   [Header("One Type Should Be A CorridorNS")]
   #endregion Header
   public bool isCorridorNS;
   // boolean value that the algorithm would use to determine the direction
   #region Header
   [Header("One Type Should Be A CorridorEW")]
   #endregion Header
   public bool isCorridorEW;
   // boolean value to identify if this is an entrance type
   #region Header
   [Header("One Type Should Be An Entrance")]
   #endregion Header
   public bool isEntrance;
   // boolean value to identify if this is a boss room type
   #region Header
   [Header("One Type Should Be A Boss Room")]
   #endregion Header
   public bool isBossRoom;
   // default none option (upon initialization of this object). Also used to track special types
   #region Header
   [Header("One Type Should Be None (Unassigned)")]
   #endregion Header
   public bool isNone;

    /// <summary>
    /// Gets called as you update a value in a scriptable object. Validates the name of SO.
    /// </summary>
   #region Validation
   #if UNITY_EDITOR
   private void OnValidate() {
    HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
   }
   #endif
   #endregion 
}
