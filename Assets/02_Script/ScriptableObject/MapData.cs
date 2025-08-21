using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    public int Rows = 10;           // 그리드의 행 수
    public int Columns = 10;
    public Vector3Int CellSize;
    public Vector3[] CellArray;
    private EventHandlers eventHandlers;
    public void Initialize(EventHandlers eventHandlers)
    {
        this.eventHandlers = eventHandlers;
        this.eventHandlers.typeEventHandler.Resgister<Vector3, bool>(typeof(MapData), IsCell);
    }

    [ContextMenu("MapGenerate")]
    public void MapGenerate()
    {
        CellArray = new Vector3[(int)(Rows * Columns)];

        for (int i = 0; i < Rows * Columns; i++)
        {
            CellArray[i] = new Vector3((CellSize.x / 2) + ((i % Rows) * CellSize.x), 0, (CellSize.z / 2) + (i / Columns * CellSize.z));
        }
    }

    public bool IsCell(Vector3 position)
    {
        foreach (var cell in CellArray)
        {
            if (cell == position)
            {
                return true;
            }
        }
        return false;
    }

}