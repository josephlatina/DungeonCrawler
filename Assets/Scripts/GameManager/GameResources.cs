/*
 * GameResources.cs
 * Author: Joseph Latina
 * Created: January 23, 2024
 * Description: Used as a centralized resource which can be accessed by all game components or scripts
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as a singleton class (only one prefab instance to exist) that can be accessed by all components or scripts
/// </summary>
public class GameResources : MonoBehaviour
{
    // This variable will hold the prefab instance of this class
    private static GameResources instance;

    public static GameResources Instance {
        // get method that retrieves the existing prefab instance
        get {
            if (instance == null) {
                // if no existing instance, load a new prefab instance
                // -- note that anything you place in Assets/Prefabs/GameResources/Resources folder can be loaded with this method
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    // PLACE ALL RESOURCES BELOW

    // A resource of type roomNodeTypeList
    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion
    // This resource is for managing the different type of room node types in the game
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Tooltip
    [Tooltip("Populate with the single instance of player's inventory")]
    #endregion
    // This resource is for managing the single instance of inventory that the player holds
    public InventorySystem inventorySystem;

    #region Tooltip
    [Tooltip("Sprite-Lit-Default Material")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Populate with the Variable Lit Shader")]
    #endregion
    public Shader variableLitShader;
    #region Tooltip
    [Tooltip("Populate with the Materialize Shader")]
    #endregion
    public Shader materializeShader;

    #region Header CHESTS
    [Space(10)]
    [Header("CHESTS")]
    #endregion
    #region Tooltip
    [Tooltip("Chest item prefab")]
    #endregion
    // This resource is holding the prefab for chest item
    public GameObject chestItemPrefab;

    #region Tooltip
    [Tooltip("Weapon prefab")]
    #endregion
    // This resource is holding the prefab for weapon
    public GameObject weaponPrefab;

    #region Tooltip
    [Tooltip("Consumable prefab")]
    #endregion
    // This resource is holding the prefab for chest item
    public GameObject consumablePrefab;

    #region Tooltip
    [Tooltip("Populate with health potion sprite")]
    #endregion
    // This resource is for chest item sprite - health potion
    public Sprite healthPotionIcon;

    #region Tooltip
    [Tooltip("Populate with weapon sprite")]
    #endregion
    // This resource is for chest item sprite - weapon
    public Sprite weaponIcon;

    #region Tooltip
    [Tooltip("Populate with pill sprite")]
    #endregion
    // This resource is for chest item sprite - pill
    public Sprite pillIcon;
}
