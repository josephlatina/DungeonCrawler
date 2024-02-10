/*
 * Room.cs
 * Author: Joseph Latina
 * Created: February 04, 2024
 * Description: Script for room objects only for the purpose of checking room overlap during Dungeon Builder algorithm (not the actual room instantiated)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the room object being placed during Dungeon Builder algorithm (not the instantiated room)
/// </summary>
public class Room
{
    // unique id of this room object
    public string id;
    // references to the room template associated with this room
    public string templateID;
    // references to the prefab for the room tilemap
    public GameObject prefab;
    // the room node type associated with this room
    public RoomNodeTypeSO roomNodeType;
    // lower and upper bound positions of the room object as it is PLACED during the dungeon builder algorithm
    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;
    // lower and upper bound positions of the original room templates that are NOT PLACED
    public Vector2Int templateLowerBounds;
    public Vector2Int templateUpperBounds;
    // the spawn position array variable defined in the room template scriptable object
    public Vector2Int[] spawnPositionArray;
    // child room ID list
    public List<string> childRoomIDList;
    // the parent room id of this room
    public string parentRoomID;
    // variable from the room template scriptable object
    public List<Doorway> doorWayList;
    // boolean value to indicate if this room has been positioned yet or not
    public bool isPositioned = false;
    // hold a reference to the actual instantiated room
    public InstantiatedRoom instantiatedRoom;
    // boolean values relating to the room
    public bool isLit = false;
    public bool isClearedOfEnemies = false;
    public bool isPreviouslyVisited = false;

    /// <summary>
    /// Constructor method for when room is created
    /// </summary>
    public Room() {
        
        childRoomIDList = new List<string>();
        doorWayList = new List<Doorway>();
    }
}
