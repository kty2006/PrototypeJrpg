using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitRegistry
{
    private readonly Dictionary<int, Unit> unitsById = new Dictionary<int, Unit>();
    private readonly List<Unit> allUnits = new List<Unit>();
    private EventHandlers eventHandlers;
    private TurnSystem turnSystem;

    public void Initialize(EventHandlers eventHandler, TurnSystem turnSystem)
    {
        this.eventHandlers = eventHandler;
        this.turnSystem = turnSystem;
        eventHandlers.typeEventHandler.Resgister<Unit>(typeof(GameInitializer), Unregister);
        eventHandlers.typeEventHandler.Resgister<TurnObject>(typeof(UnitRegistry), FindUnit);
        eventHandlers.typeEventHandler.Resgister<List<Unit>>(typeof(UnitRegistry), GetAllUnits);
        eventHandlers.typeEventHandler.Resgister<List<Vector3>, List<Unit>>(typeof(UnitRegistry), FindUnits);
        eventHandler.typeEventHandler.Resgister<int>(typeof(UnitRegistry), FindUnitById);
        eventHandler.typeEventHandler.Resgister<Vector3, TurnObject, Unit>(typeof(UnitRegistry), FindUnitAt);

    }


    public void Register(Unit unit)
    {
        if (!unitsById.ContainsKey(unit.GetInstanceID()))
        {
            unitsById.Add(unit.GetInstanceID(), unit);
            allUnits.Add(unit);
        }
    }

    public void Unregister(Unit unit)
    {
        unitsById.Remove(unit.GetInstanceID());
        allUnits.Remove(unit);
    }

    public Unit FindUnitAt(Vector3 position, TurnObject turnObject)
    {
        if (allUnits.Count == 0)
        {
            Debug.LogError("유닛이 존재하지 않음!");
            return null;
        }
        return allUnits.FirstOrDefault(unit => unit.transform.position == position && turnObject.UnitType != unit.UnitType);
    }

    public List<Unit> FindUnits(List<Vector3> gridMap)
    {
        List<Unit> foundUnits = new List<Unit>();
        foreach (var pos in gridMap)
        {
            foreach (var unit in allUnits)
            {
                if (unit.transform.position == pos)
                {
                    foundUnits.Add(unit);
                    continue;
                }
            }
        }
        return foundUnits;
    }

    public int FindUnitById()
    {
        return turnSystem.GetTurnObj().GetInstanceID();
    }

    public TurnObject FindUnit()
    {
        return turnSystem.GetTurnObj();
    }

    public List<Unit> GetAllUnits()
    {
        return allUnits;
    }
}