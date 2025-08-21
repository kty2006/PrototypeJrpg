using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PathSelectorWindow : EditorWindow
{
    private PathController selectedPathController;
    private HashSet<Vector3> currentSelectedPointsSet = new HashSet<Vector3>();
    private static HashSet<Vector3> allOccupiedPoints = new HashSet<Vector3>();

    private int gridCols = 12;

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

        EditorGUILayout.LabelField("Selected Object:", selectedObject.name, EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Grid Layout", EditorStyles.boldLabel);
        gridCols = EditorGUILayout.IntField("Columns", gridCols);
        EditorGUILayout.Separator();

        if (gridCols <= 0 || selectedPathController.sourcePathPoints == null) return;

        if (selectedPathController.sourcePathPoints.Count == 0)
        {
            EditorGUILayout.HelpBox("'Source Path Points' 리스트에 좌표를 먼저 추가해주세요.", MessageType.Warning);
            return;
        }

        UpdateCurrentSelectedPointsSet();

        // --- [최종 수정] 사용자의 마지막 예시에 맞게 정렬 순서 변경 ---
        // 1. Z값을 기준으로 내림차순 정렬 (큰 값이 먼저)
        // 2. Z값이 같으면 X값을 기준으로 오름차순 정렬 (작은 값이 먼저)
        List<Vector3> sortedPoints = selectedPathController.sourcePathPoints
            .OrderByDescending(p => Mathf.RoundToInt(p.z))
            .ThenBy(p => Mathf.RoundToInt(p.x))
            .ToList();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        int gridRows = Mathf.CeilToInt((float)sortedPoints.Count / gridCols);
        float cellHeight = (position.width - 20) / gridCols;
        float totalHeight = cellHeight * gridRows;
        Rect gridAreaRect = GUILayoutUtility.GetRect(0, 10000, totalHeight, totalHeight);
        Event e = Event.current;

        for (int i = 0; i < sortedPoints.Count; i++)
        {
            int col = i % gridCols;
            int row = i / gridCols;

            float cellWidth = gridAreaRect.width / gridCols;
            float cellX = gridAreaRect.x + col * cellWidth;
            float cellY = gridAreaRect.y + row * cellHeight;

            Rect cellRect = new Rect(cellX, cellY, cellWidth, cellHeight);
            Rect pointRect = new Rect(cellRect.center.x - pointSize / 2, cellRect.center.y - pointSize / 2, pointSize, pointSize);

            Vector3 currentPoint = sortedPoints[i];
            bool isSelectedByThis = currentSelectedPointsSet.Contains(currentPoint);
            bool isOccupiedByOther = !isSelectedByThis && allOccupiedPoints.Contains(currentPoint);

            if (isSelectedByThis) Handles.color = Color.green;
            else if (isOccupiedByOther) Handles.color = Color.red;
            else Handles.color = Color.white;

            Handles.DrawSolidDisc(pointRect.center, Vector3.forward, pointSize / 2);

            if (cellRect.Contains(e.mousePosition) && !isOccupiedByOther)
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    isDragging = true;
                    lastDraggedIndex = i;
                    dragSelectionModeIsAdding = !isSelectedByThis;
                    Undo.RecordObject(selectedPathController, "Drag Selection");
                    ToggleSelection(currentPoint, dragSelectionModeIsAdding);
                    e.Use();
                }
                else if (e.type == EventType.MouseDrag && isDragging && lastDraggedIndex != i)
                {
                    lastDraggedIndex = i;
                    ToggleSelection(currentPoint, dragSelectionModeIsAdding);
                    e.Use();
                }
            }

            GUIContent buttonContent = new GUIContent("", $"Sorted Index: {i}\nValue: {currentPoint}");
            EditorGUI.LabelField(cellRect, buttonContent);
        }

        if (e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
            lastDraggedIndex = -1;
            EditorUtility.SetDirty(selectedPathController);
        }

        EditorGUILayout.EndScrollView();
        Handles.color = Color.white;

        DrawObjectGenerationUI();
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
            if (controller == selectedPathController) continue;
            foreach (var point in controller.selectedPathPoints)
                allOccupiedPoints.Add(point);
        }
    }
    private void ToggleSelection(Vector3 point, bool shouldAdd)
    {
        bool isCurrentlySelected = currentSelectedPointsSet.Contains(point);
        if (shouldAdd && !isCurrentlySelected)
        {
            selectedPathController.selectedPathPoints.Add(point);
            UpdateCurrentSelectedPointsSet();
        }
        else if (!shouldAdd && isCurrentlySelected)
        {
            selectedPathController.selectedPathPoints.Remove(point);
            UpdateCurrentSelectedPointsSet();
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
        Repaint();
    }
    private void OnSelectionChange() { Repaint(); }
    private void OnFocus() { Repaint(); }
}