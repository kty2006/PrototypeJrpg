using Cysharp.Threading.Tasks;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public interface I_Action
{
    public Unit Unit { get; set; }

    public float StartTime { get; set; }

    public Action FuncSt { get; set; }

    public Action FuncPl { get; set; }

    public Action FuncEnd { get; set; }

    public UnitStates states { get; set; }


    public async UniTaskVoid Invoke()
    {
        FuncSt?.Invoke();

        await UniTask.WaitForSeconds(StartTime);

        Unit.Animator.SetTrigger(states.ToString());

        await UniTask.NextFrame();

        FuncPl?.Invoke();

        if (states.ToString() == "Idle" || states.ToString() == "Move" || states.ToString() == "Push")
        {
            if (Unit.Target == null || Unit.UnitType != Unit.Target.UnitType)
                FuncEnd?.Invoke();
            return;
        }

        if (await IsAction())
        {
            if (Unit.Target == null || Unit.UnitType != Unit.Target.UnitType)
                FuncEnd?.Invoke();
        }
    }

    public async UniTask<bool> IsAction()
    {
        while (true)
        {
            if(Unit != null)
                return false;
            AnimatorStateInfo stateInfo = Unit.Animator.GetNextAnimatorStateInfo(0);
            if (stateInfo.IsName("Idle"))
            {
                return true;
            }
            await UniTask.Yield();
        }
    }
}

public interface I_Object
{

}
