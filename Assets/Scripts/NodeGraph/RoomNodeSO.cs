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
    public string id;
    // used to maintain a list of parent room node IDs to reference this room node to
    public List<string> parentRoomNodeIDList = new List<string>();
    // used to maintain a list of child room node IDs to reference this room node to
    public List<string> childRoomNodeIDList = new List<string>();
    // holds a reference to the containing room node graph
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    // holds the particular room node type for this room node
    public RoomNodeTypeSO roomNodeType;
    // list of available room node types
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;


    // These methods below only pertain to within the Node Graph Editor Window:
   #region Editor Code
   #if UNITY_EDITOR
   [HideInInspector] public Rect rect; // rectangle box of the room node in the window
   [HideInInspector] public bool isLeftClickDragging = false; // boolean value to check if box is being dragged
    [HideInInspector] public bool isSelected = false; // if room node has been selected
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

        // If the room node has a parent or is of type entrance,
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance) {
            // then display a label that can't be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        // otherwise, it would be displayed as a popup
        else {
            // Display a popup using the RoomNodeType name values that can be selected from
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType); // if room node already has a selected type
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay()); // if not, check user's selection from the popup
            
            // detect the room type selection user makes and save it to the member variable within this room node SO instance
            roomNodeType = roomNodeTypeList.list[selection];
        }

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

    /// <summary>
    /// Process events for this node
    /// </summary>
   public void ProcessEvents(Event currentEvent) {
        
        switch (currentEvent.type) {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
                
            // Process Mouse Up Events (when hold on click is released)
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
   }

    /// <summary>
    /// Process mouse down events
    /// </summary>
   public void ProcessMouseDownEvent(Event currentEvent) {
        
        // left click down
        if (currentEvent.button == 0) {
            ProcessLeftClickDownEvent();
        }
        // right click down
        else if (currentEvent.button == 1) {
            ProcessRightClickDownEvent(currentEvent);
        }
   }

    /// <summary>
    /// Process left click down event
    /// </summary>
   public void ProcessLeftClickDownEvent() {
        
        // flag this object as the active object being selected in the Unity editor
        // this will enable any changes applied in the editor window to apply to the asset object it corresponds to (ie. change room node type of room)
        Selection.activeObject = this;

        // Toggle node selection
        if (isSelected) {
            isSelected = false;
        } else {
            isSelected = true;
        }
   }

    /// <summary>
    /// Process right click down event
    /// </summary>
   public void ProcessRightClickDownEvent(Event currentEvent) {
        
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
   }

    /// <summary>
    /// Process mouse up event
    /// </summary>
   public void ProcessMouseUpEvent(Event currentEvent) {
        
        // If left click has been released
        if (currentEvent.button == 0) {
            ProcessLeftClickUpEvent();
        }
   }

    /// <summary>
    /// Process left click up event
    /// </summary>
   public void ProcessLeftClickUpEvent() {
        
        if (isLeftClickDragging) {
            isLeftClickDragging = false;
        }
   }

    /// <summary>
    /// Process mouse drag event
    /// </summary>
   public void ProcessMouseDragEvent(Event currentEvent) {
        
        // process left click drag event
        if (currentEvent.button == 0) {
            ProcessLeftMouseDragEvent(currentEvent);
        }
   }

    /// <summary>
    /// Process left mouse drag event
    /// </summary>
   public void ProcessLeftMouseDragEvent(Event currentEvent) {
        
        isLeftClickDragging = true;

        DragNode(currentEvent.delta); //captures relative movement of mouse (how much it's moving by)
        GUI.changed = true;
   }

    /// <summary>
    /// Drag node
    /// </summary>
   public void DragNode(Vector2 delta) {
        
        rect.position += delta;
        EditorUtility.SetDirty(this);
   }

    /// <summary>
    /// Add childID to the node (return true if node has been added, otherwise return false)
    /// </summary>
    public bool AddChildRoomNodeIDToRoomNode(string childID) {
        
        childRoomNodeIDList.Add(childID);
        return true;
    }

    /// <summary>
    /// Add parentID to the node (return true if node has been added, otherwise return false)
    /// </summary>
    public bool AddParentRoomNodeIDToRoomNode(string parentID) {
        
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

   #endif
   #endregion Editor Code
}

