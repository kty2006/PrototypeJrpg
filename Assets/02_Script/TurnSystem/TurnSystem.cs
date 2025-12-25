using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
public class TurnSystem : IDisposable
{
    public static bool TurnProgress = false;
    private Queue<TurnObject> turnObj = new Queue<TurnObject>();
    private TurnObject currentTurnObj;
    private EventHandlers eventHandlers;
    private CancellationTokenSource cancellationTokenSource;

    public void Initialize(EventHandlers eventHandlers)
    {
        this.eventHandlers = eventHandlers;
        this.eventHandlers.typeEventHandler.Resgister<TurnObject>(typeof(TurnSystem), FastFriendly);
        this.eventHandlers.typeEventHandler.Resgister<Unit>(typeof(GameInitializer), Remove);
        turnObj.Clear();
        cancellationTokenSource = new CancellationTokenSource();

    }

    public async UniTaskVoid TurnSys(CancellationToken cancellationToken)
    {
        Sorting();
        while (true)
        {
            if (turnObj.Count == 0)
                return;

            eventHandlers.typeEventHandler.Invoke<Unit>(typeof(PlayerInformation), (Unit)FastFriendly());
            eventHandlers.typeEventHandler.Invoke<Job>(typeof(SkillText), ((Unit)FastFriendly()).Job);

            currentTurnObj = turnObj.Dequeue();
            turnObj.Enqueue(currentTurnObj); //현재 턴 오브젝트를 다시 큐에 넣음

            eventHandlers.typeEventHandler.Invoke<int>(typeof(GameInitializer), 1);

            ((Unit)currentTurnObj).IncMp(20);

            currentTurnObj.Excute().Forget();
            await UniTask.WaitUntil(() => TurnProgress, cancellationToken: cancellationToken);

            if (currentTurnObj != null)
            {
                eventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(Sorting), currentTurnObj);
                eventHandlers.typeEventHandler.Invoke<Unit>(typeof(Sorting), ((Unit)currentTurnObj));
                currentTurnObj.Target = null; //타겟 초기화
            }
            TurnProgress = false;

        }
    }

    public void Sorting()
    {
        var list = turnObj.OrderByDescending(x => x.Speed);
        turnObj = new Queue<TurnObject>(list);
        //eventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(Sorting), Sortings.AllRemove);
        foreach (var x in turnObj)
        {
            eventHandlers.typeEventHandler.Invoke<Unit>(typeof(Sorting), ((Unit)x));
        }
    }

    public void Add(TurnObject turnObject)
    {
        if (turnObject == null)
        {
            return;
        }
        turnObj.Enqueue(turnObject);
    }

    public void Remove(TurnObject turnObject)
    {
        if (turnObject == null)
        {
            return;
        }
        var list = turnObj.ToList();
        eventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(Sorting), turnObject);

        list.Remove(turnObject);
        if (currentTurnObj == turnObject)
        {
            currentTurnObj = null;
            TurnProgress = true;
        }
        turnObj = new Queue<TurnObject>(list);


        if (FastFriendly() == null)
        {
            eventHandlers.typeEventHandler.Invoke<bool>(typeof(GameEndUi), false);
            Time.timeScale = 0;
        }
        else if (FastEnemy() == null)
        {
            eventHandlers.typeEventHandler.Invoke<bool>(typeof(GameEndUi), true);
            Time.timeScale = 0;
        }
        else
            eventHandlers.typeEventHandler.Invoke<Unit>(typeof(PlayerInformation), (Unit)FastFriendly());
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return cancellationTokenSource;
    }

    public void Dispose()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
        turnObj.Clear();
    }

    public TurnObject GetTurnObj()
    {
        return currentTurnObj;
    }

    public TurnObject FastFriendly()
    {
        foreach (var turnObject in turnObj)
        {
            if (turnObject.UnitType == UnitType.Friendly)
            {
                return turnObject;
            }
        }

        return null;
    }

    public TurnObject FastEnemy()
    {
        foreach (var turnObject in turnObj)
        {
            if (turnObject.UnitType == UnitType.Enemy)
            {
                return turnObject;
            }
        }

        return null;
    }
}
