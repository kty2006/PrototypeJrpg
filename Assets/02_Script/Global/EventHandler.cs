using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class EventHandler<KeyType>
{
    public Dictionary<KeyType, EventContainer> EventDictionary = new Dictionary<KeyType, EventContainer>();

    public void Resgister<T>(KeyType key, Action<T> action)
    {
        if (!EventDictionary.ContainsKey(key))
        {
            EventDictionary.Add(key, new EventContainer());
        }
        EventDictionary[key].Resgister(new EventWrapper<T>(action));
    }

    public void UnRegisterr<T>(KeyType instanceId, EventWrapper<T> eventWrapper)
    {
        if (EventDictionary.ContainsKey(instanceId))
        {
            EventDictionary[instanceId].Unregister(eventWrapper);
        }
    }

    public void Invoke<T>(KeyType instanceId, T ev)
    {
        if (!EventDictionary.ContainsKey(instanceId))
        {
            Debug.LogError($"[EventHandler] NotRegisterEvent {nameof(KeyType)}");
        }
        EventDictionary[instanceId].Invoke(ev);
    }
}

public class EventContainer
{
    HashSet<EventWrapper> Actions = new HashSet<EventWrapper>();

    public void Resgister<T>(EventWrapper<T> eventWrapper)
    {
        Actions.Add(eventWrapper);
    }

    public void Unregister<T>(EventWrapper<T> eventWrapper)
    {
        Actions.Remove(eventWrapper);
    }

    public void Invoke<T>(T ev)
    {
        foreach (var action in Actions)
        {
            if (action is EventWrapper<T> wrapper)
            {
                wrapper.Invoke(ev);
            }
        }
    }

}

public abstract class EventWrapper
{
    public abstract void Invoke(object ev);
    public abstract bool EqualEvent(object ev);
}

public class EventWrapper<T> : EventWrapper
{
    Action<T> action;
    public override bool EqualEvent(object ev)
    {
        return true;
    }

    public override void Invoke(object ev)
    {
        if (ev is T t)
        {
            action?.Invoke(t);
        }
    }

    public EventWrapper(Action<T> ev)
    {
        action = ev;
    }
}
