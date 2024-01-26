using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedState : IState
{
    private PlayerController player;

    public PlayerRangedState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {

    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {
        player.Move();
    }

    public void Exit()
    {

    }
}
