using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapData MapData;
    public Astar astar = new Astar();
    public static Dictionary<string, int> InstancesId = new Dictionary<string, int>();

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
        Global.TurnSystem.TurnSys().Forget();
        Global.EventHandler.Resgister<int>(typeof(GameManager), RoadFind);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Global.ObjectEventHandler.Invoke(InstancesId[Global.TurnSystem.GetTurnObj().gameObject.name], UnitStates.Attack);
            Global.TurnSystem.GetTurnObj().SetState(TurnStates.Playing);
        }
    }

    public void RoadFind(int num)
    {
        astar.strPos = Vector3Int.RoundToInt(Global.TurnSystem.GetTurnObj().transform.position);
        astar.endPos = FindCell(Input.mousePosition);
        if (astar.currentTask == null)
        {
            astar.currentTask = StartCoroutine(astar.FindTarget());
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
