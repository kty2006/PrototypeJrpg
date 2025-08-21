using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathController : MonoBehaviour
{
    [Header("Path Data")]
    [Tooltip("에디터 씬 뷰에 표시될 원본 경로의 모든 지점들입니다.")]
    public List<Vector3> sourcePathPoints = new List<Vector3>();

    [Tooltip("사용자가 씬 뷰에서 클릭하여 선택한 지점들이 추가될 리스트입니다.")]
    public List<Vector3> selectedPathPoints = new List<Vector3>();

    [Tooltip("Path Editor 창에서 생성할 오브젝트의 프리팹입니다.")]
    [Header("Generation Settings")]
    public GameObject prefabToCreate; // <<<--- 이 줄이 추가되었습니다!

    [Header("Editor Settings")]
    [Tooltip("체크하면 이 컨트롤러가 선택한 점들을 다른 컨트롤러가 중복해서 선택할 수 있게 허용합니다.")]
    public bool allowOverlap = false;

    [Tooltip("Path Editor 창에 이 컨트롤러의 선택 영역이 표시될 색상입니다.")]
    public Color pathColor = Color.cyan; // <<<--- 이 줄이 추가되었습니다!

    public MapData MapData;

    public EventHandlers EventHandlers;
    public void Initialize(EventHandlers eventHandlers)
    {
        EventHandlers = eventHandlers;
        EventHandlers.typeEventHandler.Resgister<Vector3, bool>(typeof(PathController), Contanins);
    }

    [ContextMenu("SetMapData")]
    public void SetMapData()
    {
        sourcePathPoints = MapData.CellArray.ToList();
    }

    public bool Contanins(Vector3 pos)
    {
        if (selectedPathPoints.Contains(pos))
            return true;
        return false;
    }




    private void OnDrawGizmosSelected()
    {
    }
}