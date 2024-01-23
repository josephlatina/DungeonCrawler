/*
 * RoomNodeSO.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Used to create scriptable object assets for Room Node.
 */

using System.Collections;
using System.Collections.Generic;
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



}

