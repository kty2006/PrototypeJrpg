using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 모든 이벤트 Wrapper의 기반 클래스입니다.
/// </summary>
public abstract class EventWrapper
{
    public abstract object Invoke(object arg);
}

// --- 값을 반환하는 Func Wrapper ---
public class EventWrapper<TResult> : EventWrapper
{
    private readonly Func<TResult> _func;
    public EventWrapper(Func<TResult> func) { _func = func; }
    public override object Invoke(object arg) => _func();
}
public class EventWrapper<TArg, TResult> : EventWrapper
{
    private readonly Func<TArg, TResult> _func;
    public EventWrapper(Func<TArg, TResult> func) { _func = func; }
    public override object Invoke(object arg) => _func((TArg)arg);
}
public class EventWrapper<TArg1, TArg2, TResult> : EventWrapper
{
    private readonly Func<TArg1, TArg2, TResult> _func;
    public EventWrapper(Func<TArg1, TArg2, TResult> func) { _func = func; }
    public override object Invoke(object arg)
    {
        var args = ((TArg1, TArg2))arg;
        return _func(args.Item1, args.Item2);
    }
}

// --- 값을 반환하지 않는 Action Wrapper ---
public class EventWrapperAction : EventWrapper
{
    private readonly Action _action;
    public EventWrapperAction(Action action) { _action = action; }
    public override object Invoke(object arg)
    {
        _action();
        return null;
    }
}
public class EventWrapperAction<TArg> : EventWrapper
{
    private readonly Action<TArg> _action;
    public EventWrapperAction(Action<TArg> action) { _action = action; }
    public override object Invoke(object arg)
    {
        _action((TArg)arg);
        return null;
    }
}
public class EventWrapperAction<TArg1, TArg2> : EventWrapper
{
    private readonly Action<TArg1, TArg2> _action;
    public EventWrapperAction(Action<TArg1, TArg2> action) { _action = action; }
    public override object Invoke(object arg)
    {
        var args = ((TArg1, TArg2))arg;
        _action(args.Item1, args.Item2);
        return null;
    }
}


/// <summary>
/// 이벤트 Wrapper를 보관하고 실행하는 컨테이너입니다.
/// </summary>
public class EventContainer
{
    HashSet<EventWrapper> Actions = new HashSet<EventWrapper>();

    public void Resgister(EventWrapper eventWrapper) => Actions.Add(eventWrapper);

    // --- Func 호출 (Invoke) ---
    public TResult Invoke<TResult>() => (TResult)Actions.OfType<EventWrapper<TResult>>().FirstOrDefault()?.Invoke(null);
    public TResult Invoke<TArg, TResult>(TArg arg) => (TResult)Actions.OfType<EventWrapper<TArg, TResult>>().FirstOrDefault()?.Invoke(arg);
    public TResult Invoke<TArg1, TArg2, TResult>((TArg1, TArg2) args) => (TResult)Actions.OfType<EventWrapper<TArg1, TArg2, TResult>>().FirstOrDefault()?.Invoke(args);

    // --- Action 호출 (Invoke & Trigger) ---
    public void Trigger() { foreach (var wrapper in Actions.OfType<EventWrapperAction>()) wrapper.Invoke(null); }
    public void Invoke<TArg>(TArg arg) { foreach (var wrapper in Actions.OfType<EventWrapperAction<TArg>>()) wrapper.Invoke(arg); }
    public void Invoke<TArg1, TArg2>((TArg1, TArg2) args) { foreach (var wrapper in Actions.OfType<EventWrapperAction<TArg1, TArg2>>()) wrapper.Invoke(args); }
}

/// <summary>
/// 사용자가 최종적으로 상호작용하는 이벤트 시스템의 메인 클래스입니다.
/// </summary>
public class EventHandler<KeyType>
{
    public Dictionary<KeyType, EventContainer> EventDictionary = new Dictionary<KeyType, EventContainer>();

    // --- Register 메서드 ---
    public void Resgister(KeyType key, Action action)
    {
        if (!EventDictionary.ContainsKey(key)) EventDictionary.Add(key, new EventContainer());
        EventDictionary[key].Resgister(new EventWrapperAction(action));
    }
    public void Resgister<TArg>(KeyType key, Action<TArg> action)
    {
        if (!EventDictionary.ContainsKey(key)) EventDictionary.Add(key, new EventContainer());
        EventDictionary[key].Resgister(new EventWrapperAction<TArg>(action));
    }
    public void Resgister<TArg1, TArg2>(KeyType key, Action<TArg1, TArg2> action)
    {
        if (!EventDictionary.ContainsKey(key)) EventDictionary.Add(key, new EventContainer());
        EventDictionary[key].Resgister(new EventWrapperAction<TArg1, TArg2>(action));
    }
    public void Resgister<TResult>(KeyType key, Func<TResult> func)
    {
        if (!EventDictionary.ContainsKey(key)) EventDictionary.Add(key, new EventContainer());
        EventDictionary[key].Resgister(new EventWrapper<TResult>(func));
    }
    public void Resgister<TArg, TResult>(KeyType key, Func<TArg, TResult> func)
    {
        if (!EventDictionary.ContainsKey(key)) EventDictionary.Add(key, new EventContainer());
        EventDictionary[key].Resgister(new EventWrapper<TArg, TResult>(func));
    }
    public void Resgister<TArg1, TArg2, TResult>(KeyType key, Func<TArg1, TArg2, TResult> func)
    {
        if (!EventDictionary.ContainsKey(key)) EventDictionary.Add(key, new EventContainer());
        EventDictionary[key].Resgister(new EventWrapper<TArg1, TArg2, TResult>(func));
    }

    // --- 호출 메서드 ---

    // [Action, 매개변수 X] 유일하게 Trigger 사용
    public void Trigger(KeyType key)
    {
        if (EventDictionary.ContainsKey(key)) EventDictionary[key].Trigger();
    }

    // [Action, 매개변수 1개 이상] 요청대로 Invoke 사용
    public void Invoke<TArg>(KeyType key, TArg arg)
    {
        if (EventDictionary.ContainsKey(key)) EventDictionary[key].Invoke<TArg>(arg);
    }
    public void Invoke<TArg1, TArg2>(KeyType key, TArg1 arg1, TArg2 arg2)
    {
        if (EventDictionary.ContainsKey(key)) EventDictionary[key].Invoke<TArg1, TArg2>((arg1, arg2));
    }

    // [Func, 모든 경우] Invoke 사용
    public TResult Invoke<TResult>(KeyType key)
    {
        if (!EventDictionary.ContainsKey(key)) return default;
        return EventDictionary[key].Invoke<TResult>();
    }
    public TResult Invoke<TArg, TResult>(KeyType key, TArg arg)
    {
        if (!EventDictionary.ContainsKey(key)) return default;
        return EventDictionary[key].Invoke<TArg, TResult>(arg);
    }
    public TResult Invoke<TArg1, TArg2, TResult>(KeyType key, TArg1 arg1, TArg2 arg2)
    {
        if (!EventDictionary.ContainsKey(key)) return default;
        return EventDictionary[key].Invoke<TArg1, TArg2, TResult>((arg1, arg2));
    }
}