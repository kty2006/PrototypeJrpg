using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnObject : MonoBehaviour
{
    public TurnStates currentStates = TurnStates.Start;
    protected Queue<Action> actionList = new Queue<Action>();
    protected Action onTurnStart;
    public Unit Target;
    public Vector3 Pos;
    public GridMap GridMap;
    public float Speed;
    public Job Job;
    public UnitType UnitType;

    public async UniTaskVoid Excute()
    {
        Action action;
        await UniTask.WaitUntil(() => currentStates == TurnStates.Play);
        while (true)
        {
            if (currentStates == TurnStates.End)
            {
                TurnSystem.TurnProgress = true;
                currentStates = TurnStates.Start;
                return;
            }
            currentStates = TurnStates.Start;
            action = actionList.Count > 0 ? actionList.Dequeue() : null;
            action?.Invoke();
            await UniTask.WaitUntil(() => currentStates == TurnStates.Playing || currentStates == TurnStates.End);
        }
    }

    public void AddAction(Action action)
    {
        actionList.Enqueue(action);
    }

    public void AutoSetState()
    {
        currentStates = actionList.Count > 0 ? TurnStates.Playing : TurnStates.End;
    }

    public void SetState(TurnStates state)
    {
        currentStates = state;
    }

    public TurnStates GetStates()
    {
        return currentStates;
    }
}