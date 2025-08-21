using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class TurnSystem : IDisposable
{
    public static bool TurnProgress = false;
    private Queue<TurnObject> turnObj = new Queue<TurnObject>();
    private TurnObject currentTurnObj;
    private TurnObject friendlyTurnObj;
    private EventHandlers eventHandlers;

    public void Initialize(EventHandlers eventHandlers)
    {
        this.eventHandlers = eventHandlers;
        this.eventHandlers.typeEventHandler.Resgister<TurnObject>(typeof(TurnSystem), FastFriendly);
        this.eventHandlers.typeEventHandler.Resgister<Unit>(typeof(GameInitializer), Remove);

    }

    public async UniTaskVoid TurnSys()
    {
        Sorting();
        while (true)
        {
            if (turnObj.Count == 0)
                return;
            currentTurnObj = turnObj.Dequeue();
            turnObj.Enqueue(currentTurnObj); //현재 턴 오브젝트를 다시 큐에 넣음
            eventHandlers.typeEventHandler.Invoke<int>(typeof(GameInitializer), 1);


            if (currentTurnObj.UnitType == UnitType.Friendly) //스킬  ui표시
            {
                eventHandlers.typeEventHandler.Invoke<Job>(typeof(SkillText), GetTurnObj().Job);
                friendlyTurnObj = currentTurnObj;
                eventHandlers.typeEventHandler.Invoke<Unit>(typeof(PlayerInformation), (Unit)currentTurnObj);
            }
            currentTurnObj.Excute().Forget();
            await UniTask.WaitUntil(() => TurnProgress);
            eventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(Sorting), currentTurnObj);
            eventHandlers.typeEventHandler.Invoke<Unit>(typeof(Sorting), ((Unit)currentTurnObj));
            TurnProgress = false;
            currentTurnObj.Target = null; //타겟 초기화

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
        turnObj = new Queue<TurnObject>(list);
        eventHandlers.typeEventHandler.Invoke<Unit>(typeof(PlayerInformation), (Unit)FastFriendly());
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public TurnObject GetTurnObj()
    {
        return currentTurnObj;
    }

    public TurnObject FastFriendly()
    {
        if (friendlyTurnObj == null)
        {
            foreach (var turnObject in turnObj)
            {
                if (turnObject.UnitType == UnitType.Friendly)
                {
                    return turnObject;
                }
            }
        }
        return friendlyTurnObj;
    }
}
