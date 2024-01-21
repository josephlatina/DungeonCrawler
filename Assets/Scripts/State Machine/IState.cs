/*
 * IState.cs
 * Author: Josh Coss
 * Created: January 16 2024
 * Description: Interface that encapsulates logic for initializing, updating, and state transition cleanup.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State interface logic
/// </summary>
public interface IState
{
    /// <summary>
    /// Runs when first entering the state
    /// </summary>
    public void Enter() { }

    /// <summary>
    /// Per-frame logic - Include condition to transition to new state
    /// </summary>
    public void Update() { }

    /// <summary>
    /// Runs when exiting the state
    /// </summary>
    public void Exit() { }
}
