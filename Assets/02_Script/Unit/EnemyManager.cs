using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyManager
{
    protected EventHandlers eventHandlers;
    protected InputManager inputManager;
    protected UnitRegistry unitRegistry;
    protected ActionRangeSystem actionRangeSystem;
    protected TurnObject turnObject = new TurnObject();
    protected PathController holl;
    public void Initialize(EventHandlers eventHandlers, ActionRangeSystem actionRangeSystem, PathController holl)
    {
        this.eventHandlers = eventHandlers;
        this.actionRangeSystem = actionRangeSystem;
        this.holl = holl;
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
            await UniTask.WaitUntil(() => turnObject != null && turnObject.UnitType == UnitType.Enemy && turnObject.GetStates() != TurnStates.End);

            // turnObject가 null이 되거나, 턴이 끝나는 경우를 대비한 안전장치
            if (turnObject == null || turnObject.UnitType != UnitType.Enemy)
            {
                await UniTask.Yield(); // 한 프레임 대기 후 다시 확인
                continue;
            }

            eventHandlers.typeEventHandler.Invoke<int>(typeof(WaitUI), 0);
            await UniTask.WaitForSeconds(Random.Range(3, 4)); // 대기 시간 약간 줄임
            eventHandlers.typeEventHandler.Invoke<int>(typeof(WaitUI), 0);

            // 행동 선택 전, 다시 한번 현재 턴이 유효한지 확인
            if (turnObject != null && turnObject.UnitType == UnitType.Enemy && !TurnSystem.TurnProgress)
            {
                SelectAction();
                Debug.Log("삭제2");
            }
            turnObject = null;
            // 행동이 끝난 후, 다음 턴을 위해 turnObject를 null로 설정하여 중복 실행을 방지
        }
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
            unitStates = (UnitStates)UnityEngine.Random.Range((float)UnitStates.Move, (float)UnitStates.Skill2 + 1);
            eventHandlers.objectEventHandler.Invoke<UnitStates>(eventHandlers.typeEventHandler.Invoke<int>(typeof(UnitRegistry)), unitStates);
            eventHandlers.typeEventHandler.Invoke<GridMap, TurnObject>(typeof(ActionRangeSystem), turnObject.GridMap, turnObject);

            if (((int)unitStates <= (int)UnitStates.Move))
            {
                // --- 여기가 수정된 부분입니다 ---
                Vector3 targetPosition = FindBestMovePosition();
                PosSet(turnObject, targetPosition);
                // --- 여기까지 ---
                progress = false;
            }
            else
            {
                for (int i = 0; i < actionRangeSystem.GetActionGrid().Count; i++)
                {
                    var target = eventHandlers.typeEventHandler.Invoke<Vector3, TurnObject, Unit>(typeof(UnitRegistry), actionRangeSystem.GetActionGrid()[i], turnObject);

                    if (target != null && target.Job != Job.Bomb && target.UnitType != turnObject.UnitType)//캡슐화 X
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
                eventHandlers.typeEventHandler.Invoke<TurnObject, bool>(typeof(GridSystem), turnObject, false);
                eventHandlers.typeEventHandler.Invoke<GridMap, TurnObject>(typeof(ActionRangeSystem), turnObject.GridMap, turnObject);
            }

        }
    }

    /// <summary>
    /// 가장 가까운 적을 향해 이동할 최적의 위치를 찾습니다.
    /// </summary>
    /// <returns>선택된 목표 위치. 이동할 곳이 없으면 현재 위치를 반환합니다.</returns>
    private Vector3 FindBestMovePosition()
    {
        // 1. 이동 가능한 모든 위치를 가져옵니다.
        List<Vector3> movablePositions = actionRangeSystem.GetActionGrid();
        if (movablePositions == null || movablePositions.Count == 0)
        {
            return turnObject.transform.position; // 이동할 곳이 없으면 제자리에 머무릅니다.
        }

        // 2. 가장 가까운 아군 유닛을 찾습니다.
        Unit closestEnemy = FindClosestEnemy();
        if (closestEnemy == null)
        {
            // 맵에 적이 없으면, 그냥 이동 가능한 곳 중 무작위로 한 곳으로 이동합니다.
            return movablePositions[Random.Range(0, movablePositions.Count)];
        }

        // 3. 이동 가능한 모든 위치 중에서, 가장 가까운 적과 가장 가까워지는 위치를 찾습니다.
        Vector3 bestPosition = movablePositions[0];
        float minDistanceToEnemy = Vector3.Distance(bestPosition, closestEnemy.transform.position);

        foreach (var pos in movablePositions)
        {
            float currentDistance = Vector3.Distance(pos, closestEnemy.transform.position);
            if (currentDistance < minDistanceToEnemy && !holl.selectedPathPoints.Contains(pos))
            {
                minDistanceToEnemy = currentDistance;
                bestPosition = pos;
            }
        }

        return bestPosition;
    }

    /// <summary>
    /// 현재 턴의 유닛으로부터 가장 가까운 아군(Friendly) 유닛을 찾아 반환합니다.
    /// </summary>
    /// <returns>가장 가까운 아군 유닛. 맵에 아군이 없으면 null을 반환합니다.</returns>
    private Unit FindClosestEnemy()
    {
        // UnitRegistry를 통해 맵에 있는 모든 유닛 리스트를 가져옵니다.
        List<Unit> allUnits = eventHandlers.typeEventHandler.Invoke<List<Unit>>(typeof(UnitRegistry));

        Unit closestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var unit in allUnits)
        {
            // 아군(Friendly) 유닛만 대상으로 합니다.
            if (unit.UnitType == UnitType.Friendly)
            {
                float distance = Vector3.Distance(turnObject.transform.position, unit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = unit;
                }
            }
        }
        return closestEnemy;
    }


    private void PosSet(TurnObject turnObject, Vector3 pos)
    {
        turnObject.Pos = pos;
        turnObject.SetState(TurnStates.Play);
        actionRangeSystem.GetActionGrid().Clear();
    }
}