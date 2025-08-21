using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PathSelectorWindow : EditorWindow
{
    private PathController selectedPathController;
    private HashSet<Vector3> currentSelectedPointsSet = new HashSet<Vector3>();
    private static Dictionary<Vector3, PathController> allOccupiedPoints = new Dictionary<Vector3, PathController>();

    private int gridCols = 12;
    private int gridRows = 12;
    private Vector2 scrollPosition;
    private const float pointSize = 12f;
    private bool isDragging = false;
    private bool dragSelectionModeIsAdding = true;
    private int lastDraggedIndex = -1;
    private const string generatedObjectsParentName = "[Generated Objects]";

    [MenuItem("Window/Path Selector")]
    public static void ShowWindow()
    {
        GetWindow<PathSelectorWindow>("Path Selector");
    }

    private void OnGUI()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null || selectedObject.GetComponent<PathController>() == null)
        {
            EditorGUILayout.HelpBox("PathController 컴포넌트를 가진 게임 오브젝트를 선택해주세요.", MessageType.Info);
            return;
        }
        selectedPathController = selectedObject.GetComponent<PathController>();

        UpdateAllOccupiedPoints();
        UpdateCurrentSelectedPointsSet();

        EditorGUILayout.LabelField("Selected Object:", selectedObject.name, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Grid Layout", EditorStyles.boldLabel);
        gridCols = EditorGUILayout.IntField("Columns", gridCols);
        gridRows = EditorGUILayout.IntField("Rows", gridRows);
        EditorGUILayout.Separator();

        if (gridCols <= 0 || gridRows <= 0 || selectedPathController.sourcePathPoints == null) return;

        List<Vector3> sortedPoints = selectedPathController.sourcePathPoints
            .OrderByDescending(p => Mathf.RoundToInt(p.z))
            .ThenBy(p => Mathf.RoundToInt(p.x))
            .ToList();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        int maxRows = Mathf.CeilToInt((float)sortedPoints.Count / gridCols);
        if (gridRows > maxRows) gridRows = maxRows;
        float cellHeight = (position.width - 20) / gridCols;
        float totalHeight = cellHeight * gridRows;
        Rect gridAreaRect = GUILayoutUtility.GetRect(0, 10000, totalHeight, totalHeight);
        Event e = Event.current;

        for (int i = 0; i < sortedPoints.Count; i++)
        {
            int col = i % gridCols;
            int row = i / gridCols;

            if (row >= gridRows) break;

            float cellWidth = gridAreaRect.width / gridCols;
            float cellX = gridAreaRect.x + col * cellWidth;
            float cellY = gridAreaRect.y + row * cellHeight;
            Rect cellRect = new Rect(cellX, cellY, cellWidth, cellHeight);
            Rect pointRect = new Rect(cellRect.center.x - pointSize / 2, cellRect.center.y - pointSize / 2, pointSize, pointSize);

            Vector3 currentPoint = sortedPoints[i];

            bool isOccupied = allOccupiedPoints.TryGetValue(currentPoint, out PathController owner);
            bool isOwnedByThis = isOccupied && owner == selectedPathController;
            bool isLockedByOther = isOccupied && !isOwnedByThis && !owner.allowOverlap;

            // --- [최종 수정] 색상 결정 로직 ---
            if (isOccupied)
            {
                // 점이 소유되었다면, 무조건 그 소유자 컨트롤러의 pathColor로 표시
                Handles.color = owner.pathColor;
            }
            else
            {
                // 소유되지 않은 점은 흰색으로 표시
                Handles.color = Color.white;
            }

            Handles.DrawSolidDisc(pointRect.center, Vector3.forward, pointSize / 2);

            // 상호작용 로직 ('잠긴' 점은 클릭 불가)
            if (cellRect.Contains(e.mousePosition) && !isLockedByOther)
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    isDragging = true;
                    lastDraggedIndex = i;
                    dragSelectionModeIsAdding = !isOwnedByThis;

                    var controllersToRecord = new List<UnityEngine.Object> { selectedPathController };
                    if (isOccupied && !isOwnedByThis && owner != null)
                    {
                        controllersToRecord.Add(owner);
                    }
                    Undo.RecordObjects(controllersToRecord.ToArray(), "Change Point Ownership");

                    HandlePointInteraction(currentPoint, isOwnedByThis, owner, dragSelectionModeIsAdding);
                    e.Use();
                }
                else if (e.type == EventType.MouseDrag && isDragging && lastDraggedIndex != i)
                {
                    lastDraggedIndex = i;
                    HandlePointInteraction(currentPoint, isOwnedByThis, owner, dragSelectionModeIsAdding);
                    e.Use();
                }
            }

            GUIContent buttonContent = new GUIContent("", $"Index: {i}\nValue: {currentPoint}");
            EditorGUI.LabelField(cellRect, buttonContent);
        }

        if (e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
            lastDraggedIndex = -1;
            var allControllers = FindObjectsByType<PathController>(FindObjectsSortMode.None);
            foreach (var controller in allControllers)
            {
                if (controller != null) EditorUtility.SetDirty(controller);
            }
        }

        EditorGUILayout.EndScrollView();
        Handles.color = Color.white;

        DrawObjectGenerationUI();
    }

    private void HandlePointInteraction(Vector3 point, bool isOwnedByThis, PathController previousOwner, bool shouldAdd)
    {
        if (shouldAdd)
        {
            if (previousOwner != null && !isOwnedByThis)
            {
                previousOwner.selectedPathPoints.Remove(point);
            }
            if (!selectedPathController.selectedPathPoints.Contains(point))
            {
                selectedPathController.selectedPathPoints.Add(point);
            }
        }
        else
        {
            if (isOwnedByThis)
            {
                selectedPathController.selectedPathPoints.Remove(point);
            }
        }
        UpdateAllOccupiedPoints();
        UpdateCurrentSelectedPointsSet();
        Repaint();
    }

    private void DrawObjectGenerationUI()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Object Generation", EditorStyles.boldLabel);
        if (selectedPathController.prefabToCreate != null)
        {
            EditorGUILayout.LabelField("Prefab to Create:", selectedPathController.prefabToCreate.name);
        }
        else
        {
            EditorGUILayout.HelpBox("PathController의 Inspector에서 'Prefab To Create'를 지정해주세요.", MessageType.Warning);
        }
        GUI.enabled = selectedPathController.prefabToCreate != null && selectedPathController.selectedPathPoints.Count > 0;
        if (GUILayout.Button("Create Objects at Selected Points")) { CreateObjects(); }
        GUI.enabled = true;
        if (GUILayout.Button("Clear Created Objects")) { ClearObjects(); }
    }

    private void UpdateAllOccupiedPoints()
    {
        allOccupiedPoints.Clear();
        PathController[] allControllers = FindObjectsByType<PathController>(FindObjectsSortMode.None);
        foreach (var controller in allControllers)
        {
            if (controller == null) continue;
            foreach (var point in controller.selectedPathPoints)
                allOccupiedPoints[point] = controller;
        }
    }

    private void CreateObjects()
    {
        Transform parent = selectedPathController.transform.Find(generatedObjectsParentName);
        if (parent == null)
        {
            parent = new GameObject(generatedObjectsParentName).transform;
            parent.SetParent(selectedPathController.transform);
            parent.localPosition = Vector3.zero; parent.localRotation = Quaternion.identity;
            Undo.RegisterCreatedObjectUndo(parent.gameObject, "Create Generation Parent");
        }
        Undo.SetCurrentGroupName("Create Objects"); int group = Undo.GetCurrentGroup();
        foreach (Vector3 point in selectedPathController.selectedPathPoints)
        {
            GameObject prefab = selectedPathController.prefabToCreate;
            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            newObj.transform.localPosition = point;
            Undo.RegisterCreatedObjectUndo(newObj, "Create Object");
        }
        Undo.CollapseUndoOperations(group);
    }

    private void ClearObjects()
    {
        Transform parent = selectedPathController.transform.Find(generatedObjectsParentName);
        if (parent != null)
        {
            Undo.SetCurrentGroupName("Clear All Created Objects"); int group = Undo.GetCurrentGroup();
            for (int i = parent.childCount - 1; i >= 0; i--)
                Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
            Undo.CollapseUndoOperations(group);
        }
    }

    private void UpdateCurrentSelectedPointsSet()
    {
        if (selectedPathController == null) return;
        currentSelectedPointsSet = new HashSet<Vector3>(selectedPathController.selectedPathPoints);
    }

    private void OnSelectionChange() { Repaint(); }
    private void OnFocus() { Repaint(); }
}