using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("카메라 이동 설정")]
    [SerializeField]
    private float panXspeed, panYspeed = 10f;
    [SerializeField]
    private float minXpan, minYpan = 10f;
    [SerializeField]
    private float maxXpan, maxYpan = 15f;
    private float panXvalue, panYvalue = 1;

    [Header("카메라 줌 설정")]
    [SerializeField]
    private float zoomSpeed = 20f;
    private float zoomValue = 1;
    [SerializeField]
    private float minZoom = 20f;
    [SerializeField]
    private float maxZoom = 21f;


    private float savePanXspeed, savePanYspeed = 10f;

    private float saveMinXpan, saveMinYpan = 10f;

    private float saveMaxXpan, saveMaxYpan = 15f;
    private float savePanXvalue, savePanYvalue = 1;

    private float saveZoomSpeed = 20f;
    private float saveZoomValue = 1;

    private float saveMinZoom = 20f;

    private float saveMaxZoom = 21f;

    private Vector3 stPos;
    private Camera mainCamera;
    private Vector3 lastMousePosition;
    private bool isPanning = false;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        stPos = transform.position;
        savePanXspeed = panXspeed;
        savePanYspeed = panYspeed;
        saveMinXpan = minXpan;
        saveMinYpan = minYpan;
        saveMaxXpan = maxXpan;
        saveMaxYpan = maxYpan;
        savePanXvalue = panXvalue;
        savePanYvalue = panYvalue;
        saveZoomSpeed = zoomSpeed;
        saveZoomValue = zoomValue;
        saveMinZoom = minZoom;
        saveMaxZoom = maxZoom;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        HandlePan();
        HandleZoom();
    }

    private void HandlePan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }

        float movingX = Input.GetAxis("Mouse X");
        float movingY = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            panXspeed = Mathf.Clamp(panXspeed - movingX, minXpan, maxXpan);
            if (panXspeed >= maxXpan) panXvalue = 0;
            else if (panXspeed <= minXpan) panXvalue = 0;
            else panXvalue = 1;

            panYspeed = Mathf.Clamp(panYspeed - movingY, minYpan, maxYpan);
            if (panYspeed >= maxYpan) panYvalue = 0;
            else if (panYspeed <= minYpan) panYvalue = 0;
            else panYvalue = 1;

            transform.Translate(-delta.x * panXspeed * Time.deltaTime * panXvalue, -delta.y * panYspeed * Time.deltaTime * panYvalue, 0);
            lastMousePosition = Input.mousePosition;
        }
    }

    private void HandleZoom()
    {
        if (isPanning)
        {
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize -= scroll * zoomSpeed;
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                zoomSpeed = Mathf.Clamp(zoomSpeed - scroll, minZoom, maxZoom);
                if (zoomSpeed >= maxZoom) zoomValue = 0;
                else if (zoomSpeed <= minZoom) zoomValue = 0;
                else zoomValue = 1;
                transform.Translate(0, 0, scroll * zoomSpeed * zoomValue, Space.Self);
            }
        }
    }

    public void CameraReset()
    {
        transform.position = stPos;
        panXspeed = savePanXspeed;
        panYspeed = savePanYspeed;
        minXpan = saveMinXpan;
        minYpan = saveMinYpan;
        maxXpan = saveMaxXpan;
        maxYpan = saveMaxYpan;
        panXvalue = savePanXvalue;
        panYvalue = savePanYvalue;
        zoomSpeed = saveZoomSpeed;
        zoomValue = saveZoomValue;
        minZoom = saveMinZoom;
        maxZoom = saveMaxZoom;

    }
}