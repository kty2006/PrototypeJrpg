using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    public Vector2 MapSize;
    public Vector3Int CellSize;
    [SerializeField]
    public RowArray<Vector3Int>[] ColumnArray;


    [ContextMenu("MapGenerate")]
    public void MapGenerate()
    {
        ColumnArray = new RowArray<Vector3Int>[(int)MapSize.x];

        for (int x = 0; x < (int)MapSize.y; x++)
        {
            ColumnArray[x].Row = new Vector3Int[(int)MapSize.y];
        }

        for (int y = 0; y < MapSize.y; y++)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                ColumnArray[x].Row[y] = new Vector3Int((x + 1) * (CellSize.x / 2) + (x) * (CellSize.x / 2), 0, (y + 1) * (CellSize.z / 2) + (y) * (CellSize.z / 2));
            }
        }
    }
}

[Serializable]
public struct RowArray<T>
{
    public T[] Row;

    public RowArray(T[] row)
    {
        Row = row;
    }
}
