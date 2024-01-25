/*
 * RoomNodeSO.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Used to create scriptable object assets for Room Node.
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Represents the individual room node in the Room Node Graph.
/// </summary>
public class RoomNodeSO : ScriptableObject
{
    // unique guid for the room node
    [HideInInspector] public string id;
    // used to maintain a list of parent room node IDs to reference this room node to
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    // used to maintain a list of child room node IDs to reference this room node to
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    // holds a reference to the containing room node graph
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    // holds the particular room node type for this room node
    public RoomNodeTypeSO roomNodeType;
    // list of available room node types
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;


    // These methods below only pertain to within the Node Graph Editor Window:
   #region Editor Code
   #if UNITY_EDITOR
   [HideInInspector] public Rect rect;

    /// <summary>
    /// Initialise node in the Node Graph Editor window
    /// </summary>
   public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType) {
        // Initialise editor window properties
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // Load room node type list from current prefab instance of GameResources
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
   }

    /// <summary>
    /// Draw node with the node style
    /// </summary>
   public void Draw(GUIStyle nodeStyle) {
        // Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);

        // Start Region to Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();

        // Display a popup using the RoomNodeType name values that can be selected from
        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType); // if room node already has a selected type
        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay()); // if not, check user's selection from the popup
        // detect the room type selection user makes and save it to the member variable within this room node SO instance
        roomNodeType = roomNodeTypeList.list[selection];
        if (EditorGUI.EndChangeCheck()) {
            // makes sure any changes we make in the displayed popup actually gets saved
            EditorUtility.SetDirty(this);
        }

        // End of the Region
        GUILayout.EndArea();
   }

    /// <summary>
    /// Populates a string array with the room node types to display (that can be selected)
    /// </summary>
   public string[] GetRoomNodeTypesToDisplay() {
        // Populate a string array with all available options by iterating through room node type list
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++) {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor) {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
   }

   #endif
   #endregion Editor Code
}

