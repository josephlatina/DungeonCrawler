/*
 * GameManager.cs
 * Author: Joseph Latina
 * Created: February 14, 2024
 * Description: Utility script within RoomTemplateSO for dictating the spawn ratio (chance of spawning) of a spawnable object within that room
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class: For every dungeon object passed in to this, we can set the spawn ratio parameter corresponding to that spawnable object
/// </summary>
[System.Serializable]
public class SpawnableObjectRatio<T>
{
    // spawnable scriptable object passed in
    public T dungeonObject;
    // spawn ratio we can set for said object
    public int ratio;
}
