using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyManager
{
    protected EventHandlers eventHandlers;
    protected InputManager inputManager;
    protected UnitRegistry unitRegistry;
    protected ActionRangeSystem actionRangeSystem;
    protected TurnObject turnObject = new TurnObject();
    protected bool equals = false;
    public void Initialize(EventHandlers eventHandlers, ActionRangeSystem actionRangeSystem)
    {
        this.eventHandlers = eventHandlers;
        this.actionRangeSystem = actionRangeSystem;
        eventHandlers.typeEventHandler.Resgister<int>(typeof(GameInitializer), Equals);
        eventHandlers.typeEventHandler.Resgister<int>(typeof(GameInitializer), Set);

    }

    public void StAi()
    {
        Action().Forget();
    }


    public async UniTaskVoid Action()
    {
        while (true)
        {
            await UniTask.WaitUntil(() => equals && turnObject.UnitType == UnitType.Enemy);
            equals = false;
            eventHandlers.typeEventHandler.Invoke<int>(typeof(WaitUI), 0);
            await UniTask.WaitForSeconds(Random.Range(3, 5)); //µÙ∑π¿Ã
            eventHandlers.typeEventHandler.Invoke<int>(typeof(WaitUI), 0);
            SelectAction();
        }

    }
    void Equals(int i)
    {
        equals = true;
    }

    void Set(int i)
    {
        turnObject = eventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(UnitRegistry));
    }
    public void SelectAction()
    {
        UnitStates unitStates = UnitStates.Idle;
        bool progress = true;
        while (progress)
        {
            unitStates = (UnitStates)UnityEngine.Random.Range((float)UnitStates.Idle, (float)UnitStates.Skill2 + 1);
            eventHandlers.objectEventHandler.Invoke<UnitStates>(eventHandlers.typeEventHandler.Invoke<int>(typeof(UnitRegistry)), unitStates);
            eventHandlers.typeEventHandler.Invoke<GridMap, TurnObject>(typeof(ActionRangeSystem), turnObject.GridMap, turnObject);

            if (((int)unitStates <= (int)UnitStates.Move))
            {
                PosSet(turnObject, actionRangeSystem.GetActionGrid()[UnityEngine.Random.Range(0, actionRangeSystem.GetActionGrid().Count)]);
                progress = false;

            }
            else if (((Unit)turnObject).LiqStates.Mp >= ((Unit)turnObject).States.GetMpCost((UnitStates)unitStates))
            {
                for (int i = 0; i < actionRangeSystem.GetActionGrid().Count; i++)
                {
                    var target = eventHandlers.typeEventHandler.Invoke<Vector3, TurnObject, Unit>(typeof(UnitRegistry), actionRangeSystem.GetActionGrid()[i], turnObject);
                    if (target != null)
                    {
                        turnObject.Target = target;
                        PosSet(turnObject, actionRangeSystem.GetActionGrid()[i]);
                        progress = false;
                        break;
                    }
                }
            }
            if (progress)
            {
                eventHandlers.typeEventHandler.Invoke<GridMap, TurnObject>(typeof(ActionRangeSystem), turnObject.GridMap, turnObject);
            }

        }
    }

    private void PosSet(TurnObject turnObject, Vector3 pos)
    {
        turnObject.Pos = pos;
        turnObject.SetState(TurnStates.Play);
        actionRangeSystem.GetActionGrid().Clear();
    }
}
