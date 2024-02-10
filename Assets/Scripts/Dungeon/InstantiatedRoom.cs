/*
 * InstantiatedRoom.cs
 * Author: Joseph Latina
 * Created: February 09, 2024
 * Description: Script that will hold details about the instantiated room prefab
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Represents the actual room instantiated within the scene of the game
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
    /// Initialize the instantiated room
    /// </summary>
    public void Initialize(GameObject roomGameObject) {

        // populate the member variables for the tilemap and grid
        PopulateTilemapMemberVariables(roomGameObject);

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
    /// Disable the collision tilemap renderer to hide the component
    /// </summary>
    private void DisableCollisionTilemapRenderer() {

       collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
