using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapData MapData;
    public GameObject obj;
    public Astar astar = new Astar();

    public void OnDrawGizmos()
    {
        if (MapData.ColumnArray == null)
        {
            return;
        }
        for (int y = 0; y < MapData.MapSize.y; y++)
        {
            for (int x = 0; x < MapData.MapSize.x; x++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(MapData.ColumnArray[x].Row[y], MapData.CellSize);
            }
        }

    }

    public void Start()
    {
        astar.lineRenderer = GetComponent<LineRenderer>();
        obj.transform.position = MapData.ColumnArray[0].Row[0];
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //obj.transform.position = 
            astar.strPos = Vector3Int.RoundToInt(obj.transform.position);
            astar.endPos = FindCell(Input.mousePosition);
            StartCoroutine(astar.FindTarget());
        }
    }

    public Vector3Int FindCell(Vector3 position)
    {

        Vector3 pointPos;
        int index;
        List<float> posDistance = new List<float>();

        position.z = Camera.main.nearClipPlane;
        pointPos = Camera.main.ScreenToWorldPoint(position);

        for (int y = 0; y < MapData.MapSize.y; y++) //최적화 필요
        {
            for (int x = 0; x < MapData.MapSize.x; x++)
            {
                posDistance.Add(Vector3.Distance(MapData.ColumnArray[x].Row[y], pointPos));
            }
        }
        index = posDistance.IndexOf(posDistance.Min());

        return MapData.ColumnArray[index - ((index / 10) * 10)].Row[index / 10]; ;
    }

}
