using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionFactory
{
    private Dictionary<UnitStates, Func<I_Action>> factoryMap;
    public Unit unit;

    public void Initialize(Unit unit)
    {
        this.unit = unit;
        factoryMap = new Dictionary<UnitStates, Func<I_Action>>()
        {
            { UnitStates.Attack, () => new NormalAttack(this.unit,UnitStates.Attack) },
            { UnitStates.Move, () => new Move(this.unit,UnitStates.Move) },
            { UnitStates.Skill1, () => new Skill1(this.unit,UnitStates.Skill1) },
            { UnitStates.Skill2 , () => new Skill2(this.unit,UnitStates.Skill2) },
            { UnitStates.Idle, () => new Idle(this.unit,UnitStates.Idle) },
            { UnitStates.Push, () => new Push(this.unit,UnitStates.Push) }
        };
    }

    public I_Action Create(UnitStates state)
    {
        if (factoryMap.TryGetValue(state, out var factoryFunc))
        {
            return factoryFunc();
        }

        throw new ArgumentException($"[ActionFactory] No action registered for state: {state}");
    }
}