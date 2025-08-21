using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class InputManager : MonoBehaviour
{
    private EventHandlers eventHandlers;
    private UnitRegistry unitRegistry;
    private ActionRangeSystem actionRangeSystem;
    private MapData mapData;
    private TurnObject turnObject = new TurnObject();
    private CancellationTokenSource source = new();
    private int inputCount = 0;
    public Vector3 worldPos;

    public void Initialize(EventHandlers eventHandler, UnitRegistry unitManager, ActionRangeSystem actionRangeSystem, MapData mapData)//빌더 패턴으로 개선
    {
        this.eventHandlers = eventHandler;
        this.unitRegistry = unitManager;
        this.actionRangeSystem = actionRangeSystem;
        this.mapData = mapData;
    }



    //입력 받는거 플레이어 쪽으로 빼야함
    [VisibleEnum(typeof(UnitStates))]
    public void ActionInvoke(int unitStates)
    {
        turnObject = eventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(UnitRegistry));
        if (turnObject.UnitType == UnitType.Friendly && ((Unit)turnObject).LiqStates.Mp >= ((Unit)turnObject).States.GetMpCost((UnitStates)unitStates))
        {
            eventHandlers.objectEventHandler.Invoke<UnitStates>(unitRegistry.FindUnitById(), (UnitStates)unitStates);
            if ((UnitStates)unitStates != UnitStates.Idle)
            {
                if (inputCount > 0)
                {
                    actionRangeSystem.GridCheck(turnObject.GridMap, turnObject);
                }
                actionRangeSystem.GridCheck(turnObject.GridMap, turnObject);
                source?.Cancel();
                inputCount++;
                InputAwait((UnitStates)unitStates).Forget();
            }
            else
            {
                if (inputCount > 0)
                {
                    actionRangeSystem.GridCheck(turnObject.GridMap, turnObject);
                    inputCount = 0;
                }
                turnObject.Pos = turnObject.transform.position;
                turnObject.SetState(TurnStates.Play);
            }
        }
        else
        {

            eventHandlers.typeEventHandler.Invoke<int>(typeof(SkillError), 1);
        }
    }

    public async UniTask InputAwait(UnitStates unitStates)
    {
        while (true)
        {
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
            turnObject.Pos = FindCell(Input.mousePosition);
            worldPos = turnObject.Pos;
            turnObject.Target = unitRegistry.FindUnitAt(worldPos, turnObject);

            if (actionRangeSystem.GetActionGrid().Contains(worldPos))
            {
                if (UnitStates.Push > unitStates)
                { break; }
                else if (UnitStates.Push <= unitStates && turnObject.Target != null && turnObject.Target.transform.position == worldPos)
                { break; }
            }

            await UniTask.Yield();
        }
        inputCount = 0;
        turnObject.SetState(TurnStates.Play);

    }


    public Vector3 FindCell(Vector3 mousePosition)
    {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, groundLayer))
        {
            Vector3 hitPoint = hit.point;

            int index = 0;
            float minDistance = float.MaxValue;

            for (int i = 0; i < mapData.CellArray.Length; i++)
            {
                float distance = Vector3.Distance(mapData.CellArray[i], hitPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                }
            }
            return mapData.CellArray[index];
        }

        Debug.LogWarning("클릭한 위치에서 맵을 찾을 수 없습니다.");
        return Vector3.zero;
    }
}