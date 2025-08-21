using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Astar  //IDisposable 사용
{
    public Vector3 strPos;
    public Vector3 endPos;

    public Coroutine currentTask = null;

    public LineRenderer lineRenderer;

    public MapData mapData;

    private EventHandlers eventHandlers;

    public List<CellData> openList = new List<CellData>();
    public HashSet<Vector3> closedList = new HashSet<Vector3>();
    public HashSet<Vector3> wallPoses = new HashSet<Vector3>();
    public Vector3[] dir;
    public List<Unit> AllUnits;
    public PathController Holl;
    TurnObject turnObject = null; // 변수 초기화


    public Astar(MapData mapData, EventHandlers eventHandlers, LineRenderer lineRenderer, PathController holl)
    {
        dir = new Vector3[4]
        {
           new Vector3Int(mapData.CellSize.x, 0, 0), new Vector3Int(-mapData.CellSize.x, 0, 0), new Vector3Int(0, 0, mapData.CellSize.z), new Vector3Int(0, 0, -mapData.CellSize.z)
        };
        this.eventHandlers = eventHandlers;
        this.lineRenderer = lineRenderer;
        this.mapData = mapData;
        Holl = holl;
    }

    public List<Vector3> roadList = new List<Vector3>();


    public IEnumerator FindTarget()
    {
        openList.Clear();
        closedList.Clear();
        wallPoses.Clear();

        lineRenderer.positionCount = 0;
        openList.Add(new CellData(strPos, null, 0, CalculateHeuristic(strPos, endPos)));

        turnObject = eventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(UnitRegistry));

        FindWall();

        while (openList.Count > 0)
        {
            openList = openList.OrderBy(x => x.F).ThenByDescending(x => x.G).ToList();
            CellData currentCell = openList.First();
            openList.Remove(currentCell);

            if (currentCell.CurrentPos == endPos)
            {
                FillRoad(currentCell);
                break;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 neighborPos = currentCell.CurrentPos + dir[i];
                if (mapData.IsCell(neighborPos))
                {
                    if (Holl.selectedPathPoints.Contains(neighborPos) || (wallPoses.Contains(neighborPos) || closedList.Contains(neighborPos)))
                    {
                        continue;
                    }

                    float tentativeG = currentCell.G + 10;
                    float h = CalculateHeuristic(neighborPos, endPos);
                    float f = tentativeG + h;

                    CellData existingNeighbor = openList.Find(cell => cell.CurrentPos == neighborPos);

                    if (existingNeighbor != null && tentativeG >= existingNeighbor.G)
                    {
                        continue;
                    }

                    if (existingNeighbor == null)
                    {
                        openList.Add(new CellData(neighborPos, currentCell, tentativeG, h));
                        closedList.Add(currentCell.CurrentPos);
                    }
                    else
                    {
                        existingNeighbor.Parent = currentCell;
                        existingNeighbor.G = tentativeG;
                        existingNeighbor.F = f;
                    }
                }

            }
            yield return null;
        }
    }

    private void FillRoad(CellData cellData)
    {
        if (cellData.Parent != null)
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, cellData.CurrentPos);
            roadList.Add(cellData.CurrentPos);
            FillRoad(cellData.Parent);
        }
        else
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, strPos);
            RoadToEnd().Forget();
        }
    }

    public async UniTaskVoid RoadToEnd()
    {

        roadList.Reverse();

        for (int i = 0; i < AllUnits.Count; i++)
        {
            if (AllUnits[i].transform.position == endPos)
                roadList.Remove(endPos);
        }

        foreach (Vector3 road in roadList)
        {
            float time = 0;
            while (time <= 1)
            {
                turnObject.transform.position = Vector3.Lerp(turnObject.transform.position, road, time);
                time += Time.deltaTime * 4;
                await UniTask.Yield();
            }
        }
        roadList.Clear();
        currentTask = null;
        turnObject.AutoSetState();
    }

    private float CalculateHeuristic(Vector3 currentPos, Vector3 endPos)
    {
        float x = Mathf.Abs(endPos.x - currentPos.x);
        float y = Mathf.Abs(endPos.y - currentPos.y);
        float min = Mathf.Min(x, y);
        float max = Mathf.Max(x, y);
        return min * 14 + (max - min) * 10;
    }

    private void FindWall()
    {
        for (int i = 0; i < AllUnits.Count; i++)
        {
            if (AllUnits[i].transform.position == strPos)
                continue;
            if (AllUnits[i].transform.position == endPos)
                continue;
            wallPoses.Add(AllUnits[i].transform.position);
        }
    }
}


public class CellData
{
    public Vector3 CurrentPos;
    public CellData Parent;
    public float G;
    public float H;
    public float F;

    public CellData(Vector3 currentPos, CellData parent, float g, float h)
    {
        this.CurrentPos = currentPos;
        this.Parent = parent;
        this.G = g;
        this.H = h;
        this.F = g + h;
    }
}
