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
}
