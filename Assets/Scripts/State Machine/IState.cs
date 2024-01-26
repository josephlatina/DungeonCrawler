/*
 * IState.cs
 * Author: Josh Coss
 * Created: January 16, 2024
 * Description: Interface that encapsulates logic for initializing, updating, and state transition cleanup.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface defining the common logic for states.
/// </summary>
public interface IState
{
    /// <summary>
    /// Runs when first entering the state.
    /// </summary>
    void Enter();

    /// <summary>
    /// Per-frame logic. Include conditions to transition to a new state.
    /// </summary>
    void Update();

    void FixedUpdate();

    /// <summary>
    /// Runs when exiting the state.
    /// </summary>
    void Exit();
}
