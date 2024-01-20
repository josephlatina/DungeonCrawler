using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : BaseStateMachine
{
    public EnemyMoveState moveState;
    public EnemyIdleState idleState;

    public EnemyStateMachine(EnemyController enemy)
    {
        this.moveState = new EnemyMoveState(enemy);
        this.idleState = new EnemyIdleState(enemy);
    }
}
