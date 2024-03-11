/*
 * Settings.cs
 * Author: Joseph Latina
 * Created: January 25, 2024
 * Description: Used for settings
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class that can't be instantiated but used to access values pertaining to the settings
/// </summary>
public static class Settings
{
    #region DUNGEON BUILD SETTINGS
    // Max number of attempts for building the dungeon using the algorithm
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region ROOM SETTINGS
    // Max number of child corridors leading from a room
    public const int maxChildCorridors = 3; 
    #endregion

    #region GAMEOBJECT TAGS
    public const string playerTag = "Player";
    #endregion

    #region ANIMATOR PARAMETERS
    public static int use = Animator.StringToHash("use");
    #endregion

    #region ASTAR PATHFINDING PARAMETERS
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    #endregion
}
