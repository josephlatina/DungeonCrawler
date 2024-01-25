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
    public RoomNodeTypeListSO roomNodeTypeList;
}
