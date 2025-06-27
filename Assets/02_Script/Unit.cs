using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnitStates
{
    Idle,
    Move,
    Attack,
    Defend
}
public class Unit : TurnObject
{
    public States States;
    public void Awake()
    {
        GameManager.InstancesId.Add(gameObject.name, GetInstanceID());
        Global.TurnSystem.Add(this);
        States.Speed = Speed;
        Global.ObjectEventHandler.Resgister<UnitStates>(GetInstanceID(), SelectFunc);
    }


    public void SelectFunc(UnitStates states)
    {
        switch (states)
        {
            case UnitStates.Attack:
                actionList.Enqueue(OnAction);
                actionList.Enqueue(() =>
                {
                    Global.EventHandler.Invoke<int>(typeof(GameManager), 1);
                });
                break;
        }
    }

    public void OnAction()
    {
        //SetState(TurnStates.End);
    }
}
