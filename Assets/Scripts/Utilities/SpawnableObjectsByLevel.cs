/*
 * GameManager.cs
 * Author: Joseph Latina
 * Created: February 14, 2024
 * Description: Utility script within RoomTemplateSO that will contain list of spawnable objects and their spawn ratios
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class: Assigns list of spawnable objects as well as their respective spawn ratios to a room template within the assigned dungeon level
/// </summary>
[System.Serializable]
public class SpawnableObjectsByLevel<T>
{
    // dungeon level SO assigned to the room
    public DungeonLevelSO dungeonLevel;
    // list of spawnable objects as well as their respective ratios
    public List<SpawnableObjectRatio<T>> spawnableObjectRatios;
}
