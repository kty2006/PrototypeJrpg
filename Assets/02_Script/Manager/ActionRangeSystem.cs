using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ActionRangeSystem
{
    private readonly MapData mapData;
    private EventHandlers eventHandlers;
    private UnitRegistry unitRegistry;
    private List<Vector3> UnitActionGrid = new List<Vector3>();

    public void Initialize(EventHandlers eventHandlers, UnitRegistry unitRegistry)
    {
        this.eventHandlers = eventHandlers;
        this.unitRegistry = unitRegistry;
        this.eventHandlers.typeEventHandler.Resgister<GridMap, TurnObject>(typeof(ActionRangeSystem), GridCheck);
        this.eventHandlers.typeEventHandler.Resgister<List<Vector3>>(typeof(ActionRangeSystem), GetActionGrid);
    }

    public ActionRangeSystem(MapData mapData)
    {
        this.mapData = mapData;
    }

    public void GridCheck(GridMap GridMap, TurnObject turnObject)
    {
        UnitActionGrid.Clear();
        Vector3 startPos = turnObject.transform.position;
        Vector3 nextPos = new Vector3();
        UnitActionGrid.Add(startPos);

        float cellSize = mapData.CellSize.x;
        float[] xOffsets = { 0, cellSize, -cellSize };

        for (int i = 1; i < GridMap.Up; i++)
        {
            for (int x = 0; x < GridMap.Ysize; x++)
            {
                nextPos = startPos + new Vector3(xOffsets[x], 0, cellSize * i);
                if (mapData.CellArray.Contains(nextPos) && !UnitActionGrid.Contains(nextPos))
                {
                    UnitActionGrid.Add(nextPos);
                }
            }
        }

        for (int i = 1; i < GridMap.Down; i++)
        {
            for (int x = 0; x < GridMap.Ysize; x++)
            {
                nextPos = startPos + new Vector3(xOffsets[x], 0, -cellSize * i);
                if (mapData.CellArray.Contains(nextPos) && !UnitActionGrid.Contains(nextPos))
                {
                    UnitActionGrid.Add(nextPos);
                }
            }
        }

        for (int i = 1; i < GridMap.Left; i++)
        {
            for (int y = 0; y < GridMap.Xsize; y++)
            {
                nextPos = startPos + new Vector3(-cellSize * i, 0, xOffsets[y]);
                if (mapData.CellArray.Contains(nextPos) && !UnitActionGrid.Contains(nextPos))
                {
                    UnitActionGrid.Add(nextPos);
                }
            }
        }

        for (int i = 1; i < GridMap.Right; i++)
        {
            for (int y = 0; y < GridMap.Xsize; y++)
            {
                nextPos = startPos + new Vector3(cellSize * i, 0, xOffsets[y]);
                if (mapData.CellArray.Contains(nextPos) && !UnitActionGrid.Contains(nextPos))
                {
                    UnitActionGrid.Add(nextPos);
                }
            }
        }

        eventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(GridSystem), turnObject);
    }

    public List<Vector3> GetActionGrid()
    {
        return UnitActionGrid;
    }
}