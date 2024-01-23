/*
 * EnemyStateMachine.cs
 * Author: Josh Coss
 * Created: January 20 2024
 * Description: Represents the state machine for controlling enemy behavior.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the state machine for controlling enemy behavior.
/// </summary>
public class EnemyStateMachine : BaseStateMachine
{
    // States of the enemy state machine
    public EnemyMoveState moveState;
    public EnemyIdleState idleState;

    /// <summary>
    /// Constructor to initialize the enemy state machine with its states.
    /// </summary>
    public EnemyStateMachine(EnemyController enemy)
    {
        // Create instances of the states and associate them with the provided enemy controller
        this.moveState = new EnemyMoveState(enemy);
        this.idleState = new EnemyIdleState(enemy);
    }
}
