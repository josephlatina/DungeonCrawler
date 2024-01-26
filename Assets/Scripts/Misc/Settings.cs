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
    #region ROOM SETTINGS
    // Max number of child corridors leading from a room
    public const int maxChildCorridors = 3; 
    #endregion
}
