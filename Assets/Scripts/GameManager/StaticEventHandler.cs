/*
 * StaticEventHandler.cs
 * Author: Joseph Latina
 * Created: February 18, 2024
 * Description: Contains logic for implementing the event handler system
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class containing a static event that can be subscribed to by other classes/methods.
/// </summary>
public class StaticEventHandler
{
    // Room changed event that uses an Action delegate (does not return value but accepts parameters)
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    /// <summary>
    /// Method for invoking the static event 'RoomChangedEvent' with a room argument
    /// </summary>
    public static void CallRoomChangedEvent(Room room)
    {
        // pass in room object as the event argument for the room changed event
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room });
    }

    /// <summary>
    /// Method for invoking the static event 'RoomEnemiesDefeatedEvent' with a room argument
    /// </summary>
    public static event Action<RoomEnemiesDefeatedArgs> OnRoomEnemiesDefeated;

    public static void CallRoomEnemiesDefeatedEvent(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedArgs() { room = room });
    }
}

/// <summary>
/// Defines the arguments needed for an event (in this case, room object)
/// </summary>
public class RoomChangedEventArgs : EventArgs
{
    public Room room;
}

public class RoomEnemiesDefeatedArgs : EventArgs
{
    public Room room;
}
