using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Video.VideoPlayer;


public class Idle : I_Action
{
    public Idle(Unit unit, UnitStates unitStates)
    {
        Unit = unit;
        states = unitStates;
        FuncEnd += () => { Unit.AutoSetState(); };
    }
    public Unit Unit { get; set; }
    public Action FuncSt { get; set; }
    public Action FuncPl { get; set; }
    public Action FuncEnd { get; set; }
    public UnitStates states { get; set; }
    public float StartTime { get; set; }
}

public class Move : I_Action
{
    public Move(Unit unit, UnitStates unitStates)
    {
        Unit = unit;
        states = unitStates;
        FuncPl += () => { Unit.AudioSource.resource = Unit.States.GetAudio(unitStates); Unit.AudioSource.Play(); };
        FuncEnd += () => { Unit.AutoSetState(); };
    }
    public Unit Unit { get; set; }
    public Action FuncSt { get; set; }
    public Action FuncPl { get; set; }
    public Action FuncEnd { get; set; }
    public UnitStates states { get; set; }
    public float StartTime { get; set; }
}

public class Attack : I_Action
{
    public Attack(Unit unit, UnitStates unitStates)
    {
        Unit = unit;
        states = unitStates;

        Transform transform = Unit.transform;
        Quaternion rot = Quaternion.identity;

        FuncSt += () => { if (Unit.Target == null || (Unit.UnitType != Unit.Target.UnitType && Unit.Target.UnitType != UnitType.Object)) { Unit.EventHandlers.typeEventHandler.Invoke<Unit>(typeof(BattleScene), Unit); } };
        FuncSt += () => { if (Unit.Target != null || (Unit.Target.UnitType == UnitType.Object)) { rot = Unit.transform.rotation; Unit.transform.LookAt(Unit.Target.transform); } };
        FuncSt += () => { Unit.DecMP(unit.States.GetMpCost(unitStates)); };

        FuncPl += () => { Unit.Target.DecHp(Unit.LiqStates.NormalAttack * unit.States.GetScale(unitStates)); };
        FuncPl += () => { Unit.AudioSource.clip = Unit.States.GetAudio(unitStates);  Unit.AudioSource.Play();  };
        FuncPl += () => { Unit.Target.Animator.SetTrigger("Hit"); };


        FuncEnd += () => { Unit.AutoSetState(); };
        FuncEnd += () => { Unit.Target.DieCheck(); };
        FuncEnd += () => { Unit.transform.rotation = rot; };
    }
    public Unit Unit { get; set; }
    public Action FuncSt { get; set; }
    public Action FuncPl { get; set; }
    public Action FuncEnd { get; set; }
    public UnitStates states { get; set; }
    public float StartTime { get; set; }
}

public class NormalAttack : Attack
{
    public NormalAttack(Unit unit, UnitStates unitStates) : base(unit, unitStates)
    {
    }
}

public class Skill1 : Attack
{
    public Skill1(Unit unit, UnitStates unitStates) : base(unit, unitStates)
    {
    }
}

public class Skill2 : Attack
{
    public Skill2(Unit unit, UnitStates unitStates) : base(unit, unitStates)
    {
    }
}

public class Push : I_Action
{
    public Push(Unit unit, UnitStates unitStates)
    {
        Unit = unit;
        states = unitStates;
        FuncEnd += () => { Action(unit).Forget(); };
        FuncEnd += () => { Unit.AutoSetState(); };
        FuncEnd += () => { Unit.Target.Animator.SetTrigger("Hit"); };
    }
    public Unit Unit { get; set; }
    public Action FuncSt { get; set; }
    public Action FuncPl { get; set; }
    public Action FuncEnd { get; set; }
    public UnitStates states { get; set; }
    public float StartTime { get; set; }

    public async UniTaskVoid Action(Unit unit)
    {
        float time = 0;
        Vector3 pushPos = unit.Target.transform.position - unit.transform.position;
        Unit Target = unit.Target;
        pushPos += unit.Target.transform.position;
        if (unit.EventHandlers.typeEventHandler.Invoke<Vector3, TurnObject, Unit>(typeof(UnitRegistry), pushPos, Target) == null && unit.EventHandlers.typeEventHandler.Invoke<Vector3, bool>(typeof(MapData), pushPos))
        {
            while (time <= 1)
            {
                Target.transform.position = Vector3.Lerp(Target.transform.position, pushPos, time);
                time += Time.deltaTime * 4;
                await UniTask.Yield();
            }
            Target.CheckMap();
        }
    }
}
