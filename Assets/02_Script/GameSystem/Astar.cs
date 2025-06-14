using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Astar //IDisposable »ç¿ë
{
    public Vector3Int strPos;
    public Vector3Int endPos;

    public List<CellData> openList = new List<CellData>();
    public HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();
    public HashSet<Vector3Int> WallPoses = new HashSet<Vector3Int>();
    public Vector3Int[] dir = new Vector3Int[4]
    {
        new Vector3Int(2, 0, 0), new Vector3Int(-2, 0, 0), new Vector3Int(0, 0, 2), new Vector3Int(0, 0, -2)
    };

    public LineRenderer lineRenderer;


    public IEnumerator FindTarget()
    {
        openList.Clear();
        closedList.Clear();
        lineRenderer.positionCount = 0;
        openList.Add(new CellData(strPos, null, 0, CalculateHeuristic(strPos, endPos)));


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
                Vector3Int neighborPos = currentCell.CurrentPos + dir[i];

                if (WallPoses.Contains(neighborPos) || closedList.Contains(neighborPos))
                {
                    continue;
                }

                int tentativeG = currentCell.G + 10;
                int h = CalculateHeuristic(neighborPos, endPos);
                int f = tentativeG + h;

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

    private void FillRoad(CellData cellData)
    {
        if (cellData.Parent != null)
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, cellData.CurrentPos);
            FillRoad(cellData.Parent);
        }
        else
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, strPos);
        }
    }

    private int CalculateHeuristic(Vector3Int currentPos, Vector3Int endPos)
    {
        int x = Mathf.Abs(endPos.x - currentPos.x);
        int y = Mathf.Abs(endPos.y - currentPos.y);
        int min = Mathf.Min(x, y);
        int max = Mathf.Max(x, y);
        return min * 14 + (max - min) * 10;
    }
}


public class CellData
{
    public Vector3Int CurrentPos;
    public CellData Parent;
    public int G;
    public int H;
    public int F;

    public CellData(Vector3Int currentPos, CellData parent, int g, int h)
    {
        this.CurrentPos = currentPos;
        this.Parent = parent;
        this.G = g;
        this.H = h;
        this.F = g + h;
    }
}
