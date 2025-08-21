using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridSystem : MonoBehaviour
{
    public MapData MapData;
    private Mesh mesh;

    private EventHandlers eventHandlers;
    private UnitRegistry unitRegistry;

    private int totalVertices;
    private Vector3[] vertices;
    private Color[] colors;
    private List<int> changeColor = new List<int>();
    private int[] indices;

    public void Initialize(EventHandlers eventHandlers, UnitRegistry unitRegistry)
    {
        this.eventHandlers = eventHandlers;
        this.unitRegistry = unitRegistry;
        this.eventHandlers.typeEventHandler.Resgister<TurnObject>(typeof(GridSystem), SetGrid);
    }

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        int rowCount = MapData.Rows + 1;
        int colCount = MapData.Columns + 1;

        List<Vector3> vertexList = new List<Vector3>();
        List<Color> colorList = new List<Color>();
        List<int> indexList = new List<int>();

        // 가로줄 생성
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < MapData.Columns; col++)
            {
                Vector3 start = new Vector3(col * MapData.CellSize.x, 0, row * MapData.CellSize.z);
                Vector3 end = new Vector3((col + 1) * MapData.CellSize.x, 0, row * MapData.CellSize.z);

                vertexList.Add(start);
                vertexList.Add(end);
                colorList.Add(Color.blue);
                colorList.Add(Color.blue);
                indexList.Add(vertexList.Count - 2);
                indexList.Add(vertexList.Count - 1);
            }
        }

        // 세로줄 생성
        for (int col = 0; col < colCount; col++)
        {
            for (int row = 0; row < MapData.Rows; row++)
            {
                Vector3 start = new Vector3(col * MapData.CellSize.x, 0, row * MapData.CellSize.z);
                Vector3 end = new Vector3(col * MapData.CellSize.x, 0, (row + 1) * MapData.CellSize.z);

                vertexList.Add(start);
                vertexList.Add(end);
                colorList.Add(Color.blue);
                colorList.Add(Color.blue);
                indexList.Add(vertexList.Count - 2);
                indexList.Add(vertexList.Count - 1);
            }
        }

        vertices = vertexList.ToArray();
        colors = colorList.ToArray();
        int[] indices = indexList.ToArray();

        mesh = new Mesh
        {
            vertices = vertices,
            colors = colors
        };
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(TurnObject obj)
    {
        // 1. 이전에 색칠된 그리드가 있다면 파란색으로 되돌립니다.
        foreach (int i in changeColor)
        {
            if (i < colors.Length)
            {
                colors[i] = Color.blue;
            }
        }
        changeColor.Clear();

        // 2. ActionRangeSystem으로부터 정확한 공격 범위 좌표 리스트를 가져옵니다.
        List<Vector3> validCells = eventHandlers.typeEventHandler.Invoke<List<Vector3>>(typeof(ActionRangeSystem));

        // 표시할 범위가 없으면 여기서 종료합니다.
        if (validCells == null || validCells.Count == 0)
        {
            mesh.colors = colors;
            return;
        }

        // 리스트보다 빠른 조회를 위해 HashSet을 사용합니다.
        HashSet<Vector3> validCellSet = new HashSet<Vector3>(validCells);

        float halfCellX = MapData.CellSize.x / 2f;
        float halfCellZ = MapData.CellSize.z / 2f;

        // 3. 모든 그리드 선을 순회합니다.
        for (int i = 0; i < vertices.Length; i += 2)
        {
            Vector3 p1 = vertices[i];
            Vector3 p2 = vertices[i + 1];

            Vector3 adjacentCell1;
            Vector3 adjacentCell2;

            // 선이 세로선인지 가로선인지 판별하여 인접한 두 셀의 중심 좌표를 계산합니다.
            if (Mathf.Approximately(p1.x, p2.x)) // 세로선
            {
                float lineX = p1.x;
                float midZ = (p1.z + p2.z) / 2f;
                adjacentCell1 = new Vector3(lineX - halfCellX, 0, midZ);
                adjacentCell2 = new Vector3(lineX + halfCellX, 0, midZ);
            }
            else if (Mathf.Approximately(p1.z, p2.z)) // 가로선
            {
                float lineZ = p1.z;
                float midX = (p1.x + p2.x) / 2f;
                adjacentCell1 = new Vector3(midX, 0, lineZ - halfCellZ);
                adjacentCell2 = new Vector3(midX, 0, lineZ + halfCellZ);
            }
            else
            {
                continue; // 그리드 선이 아니면 건너뜁니다.
            }

            // 4. 인접한 두 셀 중 하나라도 공격 범위(validCellSet)에 포함되어 있다면, 해당 선을 붉게 칠합니다.
            if (validCellSet.Contains(adjacentCell1) || validCellSet.Contains(adjacentCell2))
            {
                colors[i] = Color.red;
                colors[i + 1] = Color.red;
                changeColor.Add(i);
                changeColor.Add(i + 1);
            }
        }

        // 5. 변경된 색상 정보를 메시에 적용합니다.
        mesh.colors = colors;
    }
}
