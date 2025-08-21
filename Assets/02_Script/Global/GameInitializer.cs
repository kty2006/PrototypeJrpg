using System;
using System.Linq;
using System.Threading;
using UnityEngine;


public class EventHandlers
{
    public EventHandler<Type> typeEventHandler = new EventHandler<Type>();
    public EventHandler<int> objectEventHandler = new EventHandler<int>();
}

public class GameInitializer : MonoBehaviour
{
    public GridSystem GridSystem;
    public MapData MapData;
    public LineRenderer PathLineRenderer;
    public TurnSystem TurnSystem = new TurnSystem();
    public BattleScene BattleScene;
    public UiManager UiManager;
    public PathController Holl;
    private static CancellationTokenSource previousTurnSystemTokenSource;

    void Awake()
    {
        Application.targetFrameRate = 120;
        if (previousTurnSystemTokenSource != null)
        {
            previousTurnSystemTokenSource.Cancel();
            previousTurnSystemTokenSource.Dispose();
        }

        Time.timeScale = 1;
        var eventHandlers = new EventHandlers();
        var enemyManager = new EnemyManager();
        var unitRegistry = new UnitRegistry();
        var pathfindingService = new Astar(MapData, eventHandlers, PathLineRenderer,Holl);
        var actionRangeSystem = new ActionRangeSystem(MapData);
        var inputManager = gameObject.GetComponent<InputManager>();

        unitRegistry.Initialize(eventHandlers, TurnSystem);
        inputManager.Initialize(eventHandlers, unitRegistry, actionRangeSystem, MapData);
        actionRangeSystem.Initialize(eventHandlers, unitRegistry);
        enemyManager.Initialize(eventHandlers, actionRangeSystem);
        MapData.Initialize(eventHandlers);
        TurnSystem.Initialize(eventHandlers);
        GridSystem.Initialize(eventHandlers, unitRegistry);
        BattleScene.Initialize(eventHandlers);
        UiManager.Initialize(eventHandlers);
        Holl.Initialize(eventHandlers);

        var skillTexts = FindObjectsByType<SkillText>(FindObjectsSortMode.None).ToList();
        skillTexts.ForEach(x => x.Initialize(eventHandlers));

        var allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None).ToList();
        foreach (var unit in allUnits)
        {
            if (!(unit is Object turnObj))
            {
                TurnSystem.Add(unit);
            }

            unit.Initialize(eventHandlers, new ActionFactory());
            eventHandlers.objectEventHandler.Resgister<UnitStates>(unit.GetInstanceID(), unit.SelectFunc);
            unitRegistry.Register(unit);
        }


        eventHandlers.typeEventHandler.Resgister<Vector3>(typeof(Astar), (targetPos) =>
        {
            pathfindingService.strPos = Vector3Int.RoundToInt(TurnSystem.GetTurnObj().transform.position);
            pathfindingService.endPos = targetPos;
            pathfindingService.AllUnits = unitRegistry.GetAllUnits();
            if (pathfindingService.currentTask == null)
            {
                StartCoroutine(pathfindingService.FindTarget());
            }
        });
        previousTurnSystemTokenSource = TurnSystem.GetCancellationTokenSource();

        TurnSystem.TurnSys(previousTurnSystemTokenSource.Token).Forget();
        enemyManager.StAi();
    }
    public void Start()
    {
    }
}