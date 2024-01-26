/*
 * RoomNodeGraphSO.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Used to create scriptable object assets for Room Node Graph.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the complete room node graph.
/// </summary>
[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    // contains complete list of room node types
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    // contains list of RoomNodeSO scriptable object assets within this room node graph SO
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    // create dictionary of RoomNodeSO with the unique guid of each room node as the key
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake() {
        LoadRoomNodeDictionary();
    }

    /// <summary>
    /// Load the room node dictionary from the room node list
    /// </summary>
    public void LoadRoomNodeDictionary() {
        // initialize the dictionary
        roomNodeDictionary.Clear();

        // Populate the dictionary with room nodes created within this room node graph SO
        foreach (RoomNodeSO node in roomNodeList) {
            roomNodeDictionary[node.id] = node;
        }
    }

    /// <summary>
    /// Get room node by room nodeID
    /// </summary>
    public RoomNodeSO GetRoomNode(string roomNodeID) {
        
        // retrieve the room node corresponding to the id from the dictionary if it exists
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode)) {
            return roomNode;
        }
        return null;
    }

    // These methods below only pertain to within the Node Graph Editor Window:
    #region Editor Code
    #if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition; // stores the end position of the drag line

    /// <summary>
    /// Repopulate node dictionary every time a change is made in the editor
    /// </summary>
    public void OnValidate() {
        
        LoadRoomNodeDictionary();
    }

    /// <summary>
    /// Takes room node and position of line and sets the member variables accordingly
    /// </summary>
    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position) {
        
        roomNodeToDrawLineFrom = node;
        linePosition = position;
    }

    #endif
    #endregion Editor Code
}
