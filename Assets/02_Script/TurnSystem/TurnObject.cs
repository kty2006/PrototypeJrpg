using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TurnStates
{
    Start,
    Playing,
    End
}
public class TurnObject : MonoBehaviour
{
    protected TurnStates currentStates = TurnStates.Start;
    protected Queue<Action> actionList = new Queue<Action>();
    public int LoadCount;
    public float Speed;
    public async UniTaskVoid Excute()
    {
        Action action;
        await UniTask.WaitUntil(() => currentStates == TurnStates.Playing);
        while (true)
        {
            if (currentStates == TurnStates.End)
            { TurnSystem.TurnProgress = true; Debug.Log("end"); return; }

            action = actionList.Count > 0 ? actionList.Dequeue() : null;
            action?.Invoke();
            await UniTask.Yield();
            //await UniTask.WaitUntil(() => currentStates == TurnStates.End);
        }
    }

    public void AddAction(Action action)
    {
        actionList.Enqueue(action);
    }

    public void SetState(TurnStates state)
    {
        currentStates = state;
    }
}
