using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{

    private readonly Dictionary<Type, List<Delegate>> listeners = new();
    public void Subscribe<T>(Action<T> handler) where T : ISimulationEvent
    {
        var type = typeof(T);

        if (!listeners.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            listeners[type] = list;
        }

        list.Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : ISimulationEvent
    {
        var type = typeof(T);
        if (listeners.TryGetValue(type, out var list))
            list.Remove(handler);
    }

    public void Raise<T>(T evt) where T : ISimulationEvent
    {
        var type = typeof(T);
        if (!listeners.TryGetValue(type, out var list))
            return;


        foreach (var del in list.ToArray())
            ((Action<T>)del)?.Invoke(evt);
    }
}
