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
        this.eventHandlers.typeEventHandler.Resgister<TurnObject,bool>(typeof(GridSystem), SetGrid);
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

        // ������ ����
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

        // ������ ����
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

    public void SetGrid(TurnObject obj, bool check)
    {
        // 1. ������ ��ĥ�� �׸��尡 �ִٸ� �Ķ������� �ǵ����ϴ�.
        if(!check)
        {
            foreach (int i in changeColor)
            {
                if (i < colors.Length)
                {
                    colors[i] = Color.blue;
                }
            }
            changeColor.Clear();
            Debug.Log(obj);
        }
        else
        {
            Debug.Log(obj);
            List<Vector3> validCells = eventHandlers.typeEventHandler.Invoke<List<Vector3>>(typeof(ActionRangeSystem));

            // ǥ���� ������ ������ ���⼭ �����մϴ�.
            if (validCells == null || validCells.Count == 0)
            {
                mesh.colors = colors;
                return;
            }

            // ����Ʈ���� ���� ��ȸ�� ���� HashSet�� ����մϴ�.
            HashSet<Vector3> validCellSet = new HashSet<Vector3>(validCells);

            float halfCellX = MapData.CellSize.x / 2f;
            float halfCellZ = MapData.CellSize.z / 2f;

            // 3. ��� �׸��� ���� ��ȸ�մϴ�.
            for (int i = 0; i < vertices.Length; i += 2)
            {
                Vector3 p1 = vertices[i];
                Vector3 p2 = vertices[i + 1];

                Vector3 adjacentCell1;
                Vector3 adjacentCell2;

                // ���� ���μ����� ���μ����� �Ǻ��Ͽ� ������ �� ���� �߽� ��ǥ�� ����մϴ�.
                if (Mathf.Approximately(p1.x, p2.x)) // ���μ�
                {
                    float lineX = p1.x;
                    float midZ = (p1.z + p2.z) / 2f;
                    adjacentCell1 = new Vector3(lineX - halfCellX, 0, midZ);
                    adjacentCell2 = new Vector3(lineX + halfCellX, 0, midZ);
                }
                else if (Mathf.Approximately(p1.z, p2.z)) // ���μ�
                {
                    float lineZ = p1.z;
                    float midX = (p1.x + p2.x) / 2f;
                    adjacentCell1 = new Vector3(midX, 0, lineZ - halfCellZ);
                    adjacentCell2 = new Vector3(midX, 0, lineZ + halfCellZ);
                }
                else
                {
                    continue; // �׸��� ���� �ƴϸ� �ǳʶݴϴ�.
                }

                // 4. ������ �� �� �� �ϳ��� ���� ����(validCellSet)�� ���ԵǾ� �ִٸ�, �ش� ���� �Ӱ� ĥ�մϴ�.
                if (validCellSet.Contains(adjacentCell1) || validCellSet.Contains(adjacentCell2))
                {
                    colors[i] = Color.red;
                    colors[i + 1] = Color.red;
                    changeColor.Add(i);
                    changeColor.Add(i + 1);
                }
            }
        }
            mesh.colors = colors;
    }
}