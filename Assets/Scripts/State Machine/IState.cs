using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void Enter()
    {
        // code that runs when first entering the state
    }

    public void Update()
    {
        // per-frame logic - include condition to transititon to new state
    }

    public void Exit()
    {
        // code that runs when exiting the state
    }
}
