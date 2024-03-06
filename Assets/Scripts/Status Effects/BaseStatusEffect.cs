using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStatusEffect
{
    public IStatus CurrentStatus { get; private set; }

    public event Action<IStatus> statusChanged;

    public void Initialize(IStatus status)
    {
        CurrentStatus = status;
        status.Enter();

        statusChanged?.Invoke(status);
    }

    public void TransitionTo(IStatus nextStatus)
    {
        CurrentStatus.Exit();
        CurrentStatus = nextStatus;
        nextStatus.Enter();

        statusChanged?.Invoke(nextStatus);
    }

    public void Update()
    {
        if (CurrentStatus != null)
        {
            CurrentStatus.Update();
        }
    }
}
