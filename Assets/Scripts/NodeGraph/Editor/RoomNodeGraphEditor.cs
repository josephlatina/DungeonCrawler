/*
 * RoomNodeGraphEditor.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Script to hold logic for building the Node Graph Editor Tool.
 */

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
    // holds reference to the current room node graph SO being opened
    private static RoomNodeGraphSO currentRoomNodeGraph;
    // holds the list of different room node types
    private RoomNodeTypeListSO roomNodeTypeList;


    //Node layout values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

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
        // Define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D; // tailor the background
        roomNodeStyle.normal.textColor = Color.white; // set text color
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding); // define padding
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder); // define border
   
        // Populate room node type list by loading the resource from the current prefab instance of GameResources
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
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
            // Process Events (ie. mouse clicks, etc)
            ProcessEvents(Event.current);

            // Draw Room Nodes
            DrawRoomNodes();
        }

        // Detect any changes in the GUI and repaint if there are any
        if (GUI.changed) {
            Repaint();
        }
        
    }

    /// <summary>
    /// Draw Editor GUI - A way for the editor to draw controls within the editor window and to detect changes
    /// </summary>
    private void ProcessEvents(Event currentEvent) {
        ProcessRoomNodeGraphEvents(currentEvent);
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
    }

    /// <summary>
    /// Show the context dropdown menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition) {
        //Create menu
        GenericMenu menu = new GenericMenu();
        //Populate menu with an action item by passing in a function
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.ShowAsContext();
    }

     /// <summary>
    /// Create a room node at the mouse position - initialize with default None if no roomNodeType passed
    /// </summary>
    private void CreateRoomNode(object mousePositionObject) {
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
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawRoomNodes() {
        // Loop through all the room nodes created within the graph and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList) {
            roomNode.Draw(roomNodeStyle);
        }

       GUI.changed = true;
    }        
}
