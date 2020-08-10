using System;
using UnityEngine.Events;

public class UnityEventHandler<Args>
    where Args : EventArgs
{
    public Args eventArgs;
    public event EventHandler<Args> eventHandler;

    public UnityEventHandler(UnityEvent unityEvent, Args eventArgs)
    {
        this.eventArgs = eventArgs;
        unityEvent.AddListener(Invoke);
    }

    public void Invoke()
    {
        eventHandler?.Invoke(this, eventArgs);
    }
}