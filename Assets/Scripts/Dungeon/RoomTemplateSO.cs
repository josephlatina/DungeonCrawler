/*
 * Doorway.cs
 * Author: Joseph Latina
 * Created: January 30, 2024
 * Description: Used to create Room Template Scriptable Object, which will hold information like prefab objects, doorway positions, spawn parameters, etc
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Represents the room template object
/// </summary>
[CreateAssetMenu(fileName = "Room_ ", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    // unique identifier for this particular room template
    [HideInInspector] public string guid;
    // used to regenerate guid if the SO is copied and prefab is changed
    [HideInInspector] public GameObject previousPrefab;

    // The room template prefab object
    #region Header ROOM PREFAB
    [Space(10)]
    [Header("ROOM PREFAB")]
    #endregion Header ROOM PREFAB
    #region Tooltip
    [Tooltip("The gameobject prefab for the room (contains all tilemaps for the room and environment game objects)")]
    #endregion Tooltip
    public GameObject prefab;

    // ROOM DETAILS
    // Hold details on which room type this SO relates to
    #region Header ROOM CONFIGURATION
    [Space(10)]
    [Header("ROOM CONFIGURATION")]
    #endregion Header ROOM CONFIGURATION
    #region Tooltip
    [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph. The exceptions being with corridors. In the room node graph there is just one corridor type 'Corridor'. For the room templates there are 2 corridor node types - CorridorNS and CorridorEW.")]
    #endregion Tooltip
    public RoomNodeTypeSO roomNodeType;

    // The bottom left corner of the rectangle around the room tilemap
    #region Tooltip
    [Tooltip("The room lower bounds represents the bottom left corner of the rectangle around the room tilemap. (Note: this is the local tilemap position and NOT world position)")]
    #endregion Tooltip
    public Vector2Int lowerBounds;

    // The upper right corner of the rectangle around the room tilemap
    #region Tooltip
    [Tooltip("The room upper bounds represents the upper right corner of the rectangle around the room tilemap. (Note: this is the local tilemap position and NOT world position)")]
    #endregion Tooltip
    public Vector2Int upperBounds;

    // The list of doorway instances existing in this room template
    #region Tooltip
    [Tooltip("There should be a max of 4 doorways for a room (one in each compass direction). There should also be a consistent 3 tile opening size with the middle tile being the doorway coordinate position")]
    #endregion Tooltip
    public List<Doorway> doorwayList;

    // Array of possible spawn positions for enemies, chests and waypoints for player
    #region Tooltip
    [Tooltip("Each possible spawn position (used for enemies and chests) should be added to this array")]
    #endregion Tooltip
    public Vector2Int[] spawnPositionArray;

    // ENEMY DETAILS
    #region Header ENEMY DETAILS
    [Space(10)]
    [Header("ENEMY DETAILS")]
    #endregion Header ENEMY DETAILS
    
    // Define the list of possible enemies that can be spawned (and spawn ratio)
    #region Tooltip
    [Tooltip("Populate the list with all enemies that can be spawned in this room by dungeon level. That includes the spawn ratio.")]
    #endregion Tooltip
    public List<SpawnableObjectsByLevel<EnemyScriptableObject>> enemiesByLevelList;

    // Define the spawn parameters of those enemies
    #region Tooltip
    [Tooltip("Populate the list with the spawn parameters for the enemies")]
    #endregion Tooltip
    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;


    /// <summary>
    /// Returns the list of Entrances for the room template
    /// </summary>
    public List<Doorway> GetDoorwayList() {
        return doorwayList;
    }

    #region Validation

    // Only to be executed in the unity editor
    #if UNITY_EDITOR

    /// <summary>
    /// Validates the SO fields
    /// </summary>
    private void OnValidate() {

        // Check if unique GUID is empty or prefab has been changed
        if (guid == "" || previousPrefab != prefab) {
            // if so, regenerate the unique GUID and set the previous prefab to the current one
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        // validate list of doorways (check if empty or have null values)
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        // validate that list of enemies are populated
        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0) {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList), roomEnemySpawnParametersList);
        }

        // validate array of the spawn positions (check if empty or have null values)
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

    #endif

    #endregion Validation
}
