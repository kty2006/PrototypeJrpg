using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Unit : TurnObject
{
    public AttackType AttackType;
    public UnitImfo States;
    public Animator Animator;
    public LiqStates LiqStates;
    public I_Action Action;
    public ActionFactory ActionFactory;
    public EventHandlers EventHandlers;
    public Rigidbody Rigidbody;
    public AudioSource AudioSource;

    public virtual void Start()
    {
        ActionFactory.Initialize(this);
        Rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Initialize(EventHandlers eventHandlers, ActionFactory actionFactory)
    {
        this.ActionFactory = actionFactory;
        this.EventHandlers = eventHandlers;
        SetStates();
        Speed = LiqStates.Speed;
        Job = States.UnitType;
    }


    public virtual void SelectFunc(UnitStates states)
    {
        Action = ActionFactory.Create(states);
        GridMap = States.GetGridMap(states);
        actionList.Clear();
        if (AttackType == AttackType.Melee || states < UnitStates.Attack)
        {
            actionList.Enqueue(() => { EventHandlers.typeEventHandler.Invoke<Vector3>(typeof(Astar), Pos); });
        }
        actionList.Enqueue(() => { EventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(GridSystem), this); AutoSetState(); });
        actionList.Enqueue(() => { Action.Invoke().Forget(); });
    }

    public void SetStates()
    {
        LiqStates.Hp = States.StHp;
        LiqStates.Speed = States.StSpeed;
        LiqStates.Mp = States.StMp;
        LiqStates.NormalAttack = States.StNormalAttack;
    }

    public void DecHp(float value)
    {
        LiqStates.Hp -= value;

    }

    public void IncHp(float value)
    {
        LiqStates.Hp += value;
        if (LiqStates.Hp > States.StHp)
        {
            LiqStates.Hp = States.StHp;
        }
    }

    public void DecMP(float value)
    {
        LiqStates.Mp -= value;

    }

    public void IncMp(float value)
    {
        LiqStates.Mp += value;
        if (LiqStates.Mp > States.StMp)
        {
            LiqStates.Mp = States.StMp;
        }
    }
    public UnitImfo GetUnitImfo()
    {
        return States;
    }

    public void DieCheck()
    {
        if (LiqStates.Hp <= 0)
        {
            Die().Forget();
        }
    }

    public async UniTaskVoid Die()
    {
        Animator.SetTrigger("Die");
        await UniTask.WaitForSeconds(Animator.GetCurrentAnimatorClipInfo(0).Length);
        EventHandlers.typeEventHandler.Invoke<Unit>(typeof(GameInitializer), this);
        Debug.Log("´ÙÀÌ");
        Destroy(gameObject);
        
    }

    public void CheckMap()
    {
        if (EventHandlers.typeEventHandler.Invoke<Vector3, bool>(typeof(PathController), this.transform.position))
        {
            Die().Forget();
        }
    }

    private void OnMouseEnter()
    {
        EventHandlers.typeEventHandler.Invoke<Unit>(typeof(StatesUI), this);

    }

    private void OnMouseExit()
    {
        EventHandlers.typeEventHandler.Invoke<Unit>(typeof(StatesUI), this);

    }
}
