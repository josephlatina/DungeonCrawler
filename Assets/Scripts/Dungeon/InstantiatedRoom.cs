/*
 * InstantiatedRoom.cs
 * Author: Joseph Latina
 * Created: February 09, 2024
 * Description: Script that will hold details about the instantiated room prefab (designed to be added as a component to room template prefabs)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Represents the actual room game object instantiated within the scene of the game
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    // references to the room object this pertains to
    [HideInInspector] public Room room;
    // reference to the grid component within the instantiated room
    [HideInInspector] public Grid grid;
    // references to the different tilemap layer components
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap wallTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    // holds info on the box collider bounds around the room
    [HideInInspector] public Bounds roomColliderBounds;
    // 2D array for storing movement penalties to be used in AStar pathfinding
    [HideInInspector] public int[,] aStarMovementPenalty;

    // reference to the box collider component
    private BoxCollider2D boxCollider2D;

    /// <summary>
    /// Called when the script is first loaded.
    /// </summary>
    private void Awake() {

        // get the box collider 2D component of the room
        boxCollider2D = GetComponent<BoxCollider2D>();

        // save the box collider bounds for the room
        roomColliderBounds = boxCollider2D.bounds;
    }

    /// <summary>
    /// Called when an object with a Collider2D component (player) enters trigger zone of another Collider2D (room) with 'is Trigger' property enabled
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the player triggered the collider
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            // Set room as visited
            this.room.isPreviouslyVisited = true;

            // Call room changed event and pass in the current room as the parameter
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    /// <summary>
    /// Initialize the instantiated room
    /// </summary>
    public void Initialize(GameObject roomGameObject) {

        // populate the member variables for the tilemap and grid
        PopulateTilemapMemberVariables(roomGameObject);

        // close off unused doorways
        BlockOffUnusedDoorWays();

        // Populate penalty array for every obstacles found in this room
        AddObstaclesAndPreferredPaths();

        // we don't want the collision tilemap layer to be displayed
        DisableCollisionTilemapRenderer();
    }

    /// <summary>
    /// Populates the tilemap and grid member variables
    /// </summary>
    private void PopulateTilemapMemberVariables(GameObject roomGameobject) {

        // Get the grid component of the instantiated room
        grid = roomGameobject.GetComponentInChildren<Grid>();

        // Get the tilemap components of that instantiated room as an array
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        // loop to populate the tilemap member variables
        foreach (Tilemap tilemap in tilemaps) {

            if (tilemap.gameObject.tag == "groundTilemap") {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap") {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap") {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "wallTilemap") {
                wallTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap") {
                collisionTilemap = tilemap;
            }
        }
    }

    /// <summary>
    /// Closes off all doorways that are unused
    /// </summary>
    private void BlockOffUnusedDoorWays() {

        // iterate through all the doorways in that room
        // note that we would have provided doorway list for each room template and room member variables mimic the room template ones
        foreach (Doorway doorway in room.doorWayList) {

            if (doorway.isConnected) {
                continue;
            }

            // for unconnected doorways, block them off using tiles on tilemaps
            if (groundTilemap != null) {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }
            if (collisionTilemap != null) {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }
            if (decoration1Tilemap != null) {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null) {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
            if (wallTilemap != null) {
                BlockADoorwayOnTilemapLayer(wallTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Close off a certain doorway on a tilemap layer
    /// </summary>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway) {

        switch (doorway.orientation) {

            // block doorways horizontally
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            // block doorways vertically
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.none:
                break;
        }
    }

    /// <summary>
    /// Block doorway horizontally for north and south doorways
    /// </summary>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway) {

        // get the start position of where the doorway will be blocked off
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // outer loop to go through all tiles on the xaxis
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++) {
            // inner loop to go through all tiles on yaxis
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++) {

                // Get the transformation matrix of tile being copied (rotation + transformation)
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Copy same tile onto the next tile over
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Apply same transformation matrix on copied tile
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    } 

    /// <summary>
    /// Block doorway vertically for east and south doorways
    /// </summary>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway) {

        // get the start position of where the doorway will be blocked off
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // outer loop to go through all tiles on the yaxis
       for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++) {
            // inner loop to go through all tiles on xaxis
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++) {

                // Get the transformation matrix of tile being copied (rotation + transformation)
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Copy same tile onto the next tile over
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Apply same transformation matrix on copied tile
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    } 

    /// <summary>
    /// Add penalty for every obstacles found in the room for AStar pathfinding (pentalty of 0 for every tile that is unwalkable)
    /// </summary>
    private void AddObstaclesAndPreferredPaths()
    {
        // this penalty array will be populated with wall obstacles 
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        // Loop through all grid squares in this room
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                // Set default movement penalty for grid sqaures
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                // Add obstacles for collision tiles the enemy can't walk on
                // get the collision tilemap tile at the specified grid position
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                // check if this tile is one of the unwalkable collision tiles array we've added in the game resources
                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        // if so, then set the penalty to 0
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                // Add preferred path for enemies (1 is the preferred path value, default value for
                // a grid location is specified in the Settings).
                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }

            }
        }

    }

    /// <summary>
    /// Disable the collision tilemap renderer to hide the component
    /// </summary>
    private void DisableCollisionTilemapRenderer() {

       collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
