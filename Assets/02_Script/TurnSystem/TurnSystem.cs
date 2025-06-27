using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TurnSystem : IDisposable
{
    public static bool TurnProgress = false;
    private Queue<TurnObject> turnObj = new Queue<TurnObject>();
    private TurnObject currentTurnObj;
    public async UniTaskVoid TurnSys()
    {
        Sorting();
        while (true)
        {
            if (turnObj.Count == 0)
                return;
            currentTurnObj = turnObj.Dequeue();
            currentTurnObj.Excute().Forget();
            Debug.Log($"{currentTurnObj.name}+ {turnObj.Count}");
            await UniTask.WaitUntil(() => TurnProgress);
            TurnProgress = false;
        }
    }

    public void Sorting()
    {
        turnObj.OrderByDescending(x => x.Speed);
    }

    public void Add(TurnObject turnObject)
    {
        if (turnObject == null)
        {
            Debug.LogError("TurnObject is null");
            return;
        }
        turnObj.Enqueue(turnObject);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public TurnObject GetTurnObj()
    {
        return currentTurnObj;
    }
}
