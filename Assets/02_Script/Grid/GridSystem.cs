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
        indices = indexList.ToArray();
        totalVertices = vertices.Length;

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
        if (changeColor.Count > 0)
        {
            foreach (int i in changeColor)
            {
                colors[i] = Color.blue; // 초기화
            }
            changeColor.Clear();

        }
        else
        {
            Vector3 center = obj.transform.position;
            float range = MapData.CellSize.x * Mathf.Max(obj.GridMap.Up, obj.GridMap.Down, obj.GridMap.Left, obj.GridMap.Right);

            for (int i = 0; i < vertices.Length; i += 2)
            {
                Vector3 midPoint = (vertices[i] + vertices[i + 1]) / 2f;
                float distance = Vector3.Distance(midPoint, center);

                if (distance <= range)
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
