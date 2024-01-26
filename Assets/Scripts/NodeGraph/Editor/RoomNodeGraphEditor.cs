/*
 * RoomNodeGraphEditor.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Script to hold logic for building the Node Graph Editor Tool.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;

/// <summary>
/// Builds the Editor Window for conveniently creating different node graph objects.
/// </summary>
public class RoomNodeGraphEditor : EditorWindow
{
    // holds the node layout style we are using for the editor window
    private GUIStyle roomNodeStyle;
    // holds the node layout style for a node that has been selected
    private GUIStyle roomNodeSelectedStyle;
    // holds reference to the current room node graph SO being opened
    private static RoomNodeGraphSO currentRoomNodeGraph;

    // holds the amount of drag distance of mouse
    private Vector2 graphDrag;

    // holds reference to the current room node being hovered over by mouse
    private RoomNodeSO currentRoomNode = null;
    // holds the list of different room node types
    private RoomNodeTypeListSO roomNodeTypeList;


    //Node layout values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Connecting line values
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    /// <summary>
    /// Method to open an editor window
    /// </summary>
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow() {
        // returns the EditorWindow on the screen
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    /// <summary>
    /// Method called when the editor window is enabled
    /// </summary>
    private void OnEnable() {

        // Allows unity to change editor window based on node graph selected by user in project hierarchy
        Selection.selectionChanged += InspectorSelectionChanged;

        // Define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D; // tailor the background
        roomNodeStyle.normal.textColor = Color.white; // set text color
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding); // define padding
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder); // define border
   
        // Define selected node style
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D; // tailor the background
        roomNodeSelectedStyle.normal.textColor = Color.white; // set text color
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding); // define padding
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder); // define border

        // Populate room node type list by loading the resource from the current prefab instance of GameResources
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
   }

    /// <summary>
    /// Method called when the editor window is disabled
    /// </summary>
    private void OnDisable() {
        // Allows unity to change editor window based on node graph selected by user in project hierarchy
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

     /// <summary>
    /// If a room node graph scriptable object asset is double clicked in the inspector, open the room node graph editor window 
    /// </summary>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line) {
        // pass the instance id from the object and check if it is of type RoomNodeGraphSO
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        // if it is, open window
        if (roomNodeGraph != null) {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Draw Editor GUI - A way for the editor to draw controls within the editor window and to detect changes
    /// </summary>
    private void OnGUI() {
        // Only process if a scriptable object of type RoomNodeGraphSO has been selected
        if (currentRoomNodeGraph != null) {

            // Draw a line first if mouse is being dragged (so it appears behind nodes that are drawn after)
            DrawDraggedLine();

            // Process Events (ie. mouse clicks, etc)
            ProcessEvents(Event.current);

            // Draw the connection line between two room nodes
            DrawRoomConnections();

            // Draw Room Nodes
            DrawRoomNodes();
        }

        // Detect any changes in the GUI and repaint if there are any
        if (GUI.changed) {
            Repaint();
        }
    }

    /// <summary>
    /// Draw the dragged line
    /// </summary>
    private void DrawDraggedLine() {
        
        // if ending line position is not empty,
        if (currentRoomNodeGraph.linePosition != Vector2.zero) {
            // Draw a line from room node to the ending line position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    /// <summary>
    /// Draw Editor GUI - A way for the editor to draw controls within the editor window and to detect changes
    /// </summary>
    private void ProcessEvents(Event currentEvent) {

        // if current room node is null or mouse is not currently dragging any room node,
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false) {
            // then check if mouse is hovering over a particular room node and return it
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // if mouse is still not hovering over any room node or we are currently trying to drag down a line
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null) {
            // then process all of the events in this graph
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        // or if mouse is hovering over one,
        else {
            // then process the room node events
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    /// <summary>
    /// Checks to see if the mouse is over a room node and if so, then return the room node
    /// </summary>
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent) {

        // If the mouse position falls within the area of the rect for a certain room node, then return it
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--) {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition)) {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        // Otherwise return  null
        return null;
    }

    /// <summary>
    /// Checks what current event type has been detected and process it
    /// </summary>
    private void ProcessRoomNodeGraphEvents(Event currentEvent) {
        switch (currentEvent.type) {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Event
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            
            default:
                break;
        }
    }

    /// <summary>
    /// Processes mouse down events on the room node graph
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent) {
        
        // Process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1) {
            ShowContextMenu(currentEvent.mousePosition);
        }
        // Process left mouse down on graph event (clear any lines/selected nodes when clicking on canvas)
        else if (currentEvent.button == 0) {
            // clear any line drags in process
            ClearLineDrag();
            // clear any selected room nodes
            ClearAllSelectedRoomNodes();
        }
    }

    /// <summary>
    /// Show the context dropdown menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition) {
        //Create menu
        GenericMenu menu = new GenericMenu();

        //Populate menu with an action item by passing in a function
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        //Select all nodes
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        //Delete selected nodes
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddSeparator("");
        //Delete selected nodes
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

     /// <summary>
    /// Create a room node at the mouse position - aside from entrance room, any other room is initialized with default None if no roomNodeType passed
    /// </summary>
    private void CreateRoomNode(object mousePositionObject) {

        // If current node graph is empty, then automatically make the first created node to be the entrance room
        if (currentRoomNodeGraph.roomNodeList.Count == 0) {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// Create a room node at the mouse position - overloaded to also pass in RoomNodeType
    /// </summary>
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType) {
        Vector2 mousePosition = (Vector2)mousePositionObject; 

        // create a room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // add the created room node to the current room node graph node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // initialize the room node box within the window
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // add room node to the database for room node graph scriptable object asset
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();

        // Refresh the node graph dictionary for current node graph
        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// Delete all of the selected room nodes
    /// </summary>
    private void DeleteSelectedRoomNodes() {
        
        // Create a queue collection to store any room nodes we want to delete
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        // Loop through all of the nodes in the graph and queue the selected ones
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            // If room node is selected and it's not an entrance,
            if (roomNode.isSelected & !roomNode.roomNodeType.isEntrance) {
                // Then store it in our queue
                roomNodeDeletionQueue.Enqueue(roomNode);

                // Now for each child room node id of this room node, remove the links
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList) {
                    // retrieve the corresponding child room node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);
                    // if child room node is found,
                    if (childRoomNode != null) {
                        // then remove the parentID link from this child node to current room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
                
                // Now iterate through all of the parent room node ids of this room node and remove the links
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList) {
                    // retrieve the corresponding parent room node
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);
                    // If parent room node is found,
                    if (parentRoomNode != null) {
                        // then remove the childID link from the parentNode to this room node
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // Delete the queued room nodes
       while (roomNodeDeletionQueue.Count > 0) {
            
            // Get the room node to delete from the queue
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();
            // Remove the node from the graph's dictionary
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            // And remove the node from the graph's list
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            // Remove the room node scriptable object from the Asset database
            DestroyImmediate(roomNodeToDelete, true);
            // Save the asset database
            AssetDatabase.SaveAssets();
       }
    }

    /// <summary>
    /// Delete all of the selected room node links
    /// </summary>
    private void DeleteSelectedRoomNodeLinks() {
        
        // Iterate through all of the room nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            // if room node is selected and has children
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0) {
                // iterate through its child room node ids
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--) {
                    // Get the corresponding child room node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    // If the child room node is also selected, remove the links
                    if (childRoomNode != null && childRoomNode.isSelected) {
                        // Do so by first removing the childID from the parent room node
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        // and remove the parentID from the child room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // Clear all of the selected room nodes
        ClearAllSelectedRoomNodes();
    }

    /// <summary>
    /// Clear selection from all room nodes
    /// </summary>
    private void ClearAllSelectedRoomNodes() {
        
        // Iterate through all existing room nodes within graph
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            // reset any room nodes that has been selected
            if (roomNode.isSelected) {
                roomNode.isSelected = false;
                // repaint the window
                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// Select all room nodes
    /// </summary>
    private void SelectAllRoomNodes() {
        
        // Iterate through all existing room nodes within graph
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    /// <summary>
    /// Processes mouse up events on the room node graph
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent) {
        
        // If you are releasing the right mouse button after dragging a line
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null) {
            // Check if you are over a room node
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
            // If you are, 
            if (roomNode != null) {
                // then try to set it as a child of the parent room node
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id)) {
                    // if successful, then set the parent ID in child room node
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }
            // If not, then simply clear the line
            ClearLineDrag();
        }
    }

    /// <summary>
    /// Process mouse drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent) {
        // process right click drag event (drawing a line)
        if (currentEvent.button == 1) {
            ProcessRightMouseDragEvent(currentEvent);
        }
        // process left click drag event (dragging node graph)
        else if (currentEvent.button == 0) {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    /// <summary>
    /// Process right mouse drag event - drawing a line
    /// </summary>
    private void ProcessRightMouseDragEvent(Event currentEvent) {
        
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null) {
            //check how much you've moved the mouse over by
            DragConnectingLine(currentEvent.delta);
            //repaint the window in order to call the function that actually draws the line based on the ending line position
            GUI.changed = true;
        }
    }

    /// <summary>
    /// Process left mouse drag event - dragging node graph
    /// </summary>
    private void ProcessLeftMouseDragEvent(Vector2 dragDelta) {
        
        graphDrag = dragDelta;
        
        // Iterate through room nodes in graph
        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++) {
            // Drag each node by the same distance moved by mouse
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        // repaint window
        GUI.changed = true;
    }

    /// <summary>
    /// Keep track of where the dragged mouse stops for drawing the line
    /// </summary>
    private void DragConnectingLine(Vector2 delta) {

        //set the ending position of the drag line by the distance covered by mouse
        currentRoomNodeGraph.linePosition += delta;
    }

    /// <summary>
    /// Clear line drag from a room node
    /// </summary>
    private void ClearLineDrag() {

        // reset the line values
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    /// <summary>
    /// Draw connection line in the graph window between the two room nodes
    /// </summary>
    private void DrawRoomConnections() {

        // Loop through all existing room nodes within the graph
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            // if current room node has a child room node,
            if (roomNode.childRoomNodeIDList.Count > 0) {
                // Loop through all the child room node ids
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList) {
                    // and retrieve the child room node object from dictionary based on given room id
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID)) {
                        // then draw the connection line from this node to its corresponding child node
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
                        // repaint the window
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draw connection line between the parent room node and the child room node
    /// </summary>
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode) {
        
        // get line start and end position
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        // calculate midway point
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        // Vector from start to end position of line
        Vector2 direction = endPosition - startPosition;

        // Calculate normalised perpendicular positions from midpoint
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        // Calculate mid point offset position for arrow head
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        // Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        // draw the line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        // repaint the window
        GUI.changed = true;
    }     

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawRoomNodes() {
        // Loop through all the room nodes created within the graph and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            // if room node has been selected,
            if (roomNode.isSelected) {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            // otherwise,
            else {
                roomNode.Draw(roomNodeStyle);
            }
        }
        // repaint the window
       GUI.changed = true;
    }

    /// <summary>
    /// Selection changed in the project hierarchy (gets called whenever unity detects an item selection change in project hierarchy)
    /// </summary>
    private void InspectorSelectionChanged() {
        
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null) {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }   
}
