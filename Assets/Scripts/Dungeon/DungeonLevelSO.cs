/*
 * DungeonLevelSO.cs
 * Author: Joseph Latina
 * Created: February 01, 2024
 * Description: Script for creating dungeon level scriptable object
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the dungeon level that contains linked room nodes
/// </summary>
[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject {
    
    // The name for the level
    #region Header BASIC LEVEL DETAILS
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    #endregion Header BASIC LEVEL DETAILS
    #region Tooltip
    [Tooltip("The name for the level")]
    #endregion Tooltip
    public string levelName;

    // The list of room templates to be used for this level
    #region Header ROOM TEMPLATES FOR LEVEL
    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    #endregion Header ROOM TEMPLATES FOR LEVEL
    #region Tooltip
    [Tooltip("The name for the levelPopulate the list with room templates you want to be part of the level and that there's a template for each room node type specified in room node graph")]
    #endregion Tooltip
    public List<RoomTemplateSO> roomTemplateList;

    // The list of room node graphs to be chosen out of for this level
    #region Header ROOM NODE GRAPHS FOR LEVEL
    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    #endregion Header ROOM NODE GRAPHS FOR LEVEL
    #region Tooltip
    [Tooltip("Populate this list with room node graphs to be randomly selected from for this level")]
    #endregion Tooltip
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation

    #if UNITY_EDITOR

    /// <summary>
    /// Validates dungeon level scriptable object details entered
    /// </summary>
    private void OnValidate() {

        // 1. INITIAL VALIDATIONS

        // Validate string for level name
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        // Validate that list of room templates is not empty or have null values
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList)) {
            // if error is returned as true, then exit out of function
            return;
        }
        // Validate that list of room node graphs is not empty or have null values
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList)) {
            // if error is returned as true, then exit out of function
            return;
        }

        // 2. VALIDATE ROOM TEMPLATES

        // Check to make sure that room templates are specified for each room node type in node graph
        // Initialize variables to check that NS corridor, EW corridor and entrance types have been specified
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;
        // Loop through all room templates to check that the given room node type has been specified
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList) {
            
            // check if room template is null
            if (roomTemplateSO == null) {
                return;
            }

            // check for room type of the given room template and set our variables accordingly
            if (roomTemplateSO.roomNodeType.isCorridorEW) {
                isEWCorridor = true;
            }
            if (roomTemplateSO.roomNodeType.isCorridorNS) {
                isNSCorridor = true;
            }
            if (roomTemplateSO.roomNodeType.isEntrance) {
                isEntrance = true;
            }
        }

        // Print debug logs accordingly for missing room node types
        if (isEWCorridor == false) {
            Debug.Log("In " + this.name.ToString() + " : No EW Corridor Room Type Specified.");
        }
        if (isNSCorridor == false) {
            Debug.Log("In " + this.name.ToString() + " : No NS Corridor Room Type Specified.");
        }
        if (isEntrance == false) {
            Debug.Log("In " + this.name.ToString() + " : No Entrance Room Type Specified.");
        }

        // 3. VALIDATE NODE GRAPHS

        // Loop through all of the node graphs
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList) {

            // check that the room node graph is not null
            if (roomNodeGraph == null) {
                return;
            }

            // loop through all of the room nodes
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList) {

                // if this room node is null, continue to the next node
                if (roomNodeSO == null) {
                    continue;
                }

                // check that room template has been specified for each room node type
                // skip the corridors and entrance as those have already been checked - there's already a template for them
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone) {
                    continue;
                }

                // initialize flag
                bool isRoomNodeTypeFound = false;
                // loop through the room templates to check that there is one matching room type for this given node
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList) {

                    // if this room node is null, continue to the next node
                    if (roomNodeSO == null) {
                        continue;
                    }

                    // if there is a match, flag it and break out of loop
                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType) {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                    
                }

                // If no match, set a message
                if (!isRoomNodeTypeFound) {
                    Debug.Log("In " + this.name.ToString() + " : No room template for " + roomNodeSO.roomNodeType.name.ToString() + " found for node graph " + roomNodeGraph.name.ToString());
                }
            }
        }
    }


    #endif

    #endregion Validation
}

