using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, PlayerControls.IPlayerActions
{
    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 1f;
    

    [Header("Zoom Settings")]
    [SerializeField] private float zoomMin = 3f;
    [SerializeField] private float zoomMax = 10f;
    [SerializeField] private float zoomSpeed = 0.5f;

    [Header("Camera Bounds")]
    [SerializeField] private float leftBound = -10f;
    [SerializeField] private float rightBound = 20f;

    [Header("Fixed Y and Z")]
    [SerializeField] private float fixedY = 0f;
    [SerializeField] private float fixedZ = -10f;
    
    private PlayerControls controls;
    [SerializeField] private CameraController cameraController;

    private Camera cam;
    private bool isDragging;
    private Vector2 lastMousePos;
    private Vector2 currentMousePos;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
    }

    private void LateUpdate()
    {
        if (isDragging)
        {
            HandleDragging();
        }
        ClampCameraX();
    }

    private void OnEnable()
    {
        controls.Enable();
        CameraBoundsEvents.OnCameraBoundsChanged += SetCameraBounds;

    }

    private void OnDisable()
    {
        controls.Disable();
        CameraBoundsEvents.OnCameraBoundsChanged -= SetCameraBounds;

    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.started)
            cameraController.StartDragging();
        else if (context.canceled)
            cameraController.StopDragging();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        Vector2 scroll = context.ReadValue<Vector2>();
        cameraController.HandleZoom(scroll.y);
    }

    // Called by CameraInputProxy indirectly
    public void StartDragging()
    {
        isDragging = true;
        lastMousePos = currentMousePos;
    }

    public void StopDragging()
    {
        isDragging = false;
    }

    private void HandleDragging()
    {
        Vector2 currentPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Vector2 delta = currentPos - lastMousePos;
        lastMousePos = currentPos;

        float dragFactor = cam.orthographicSize / 5f;
        Vector3 move = new Vector3(-delta.x * dragSpeed * dragFactor * Time.deltaTime, 0f, 0f);
        transform.position += move;
    }

    public void HandleZoom(float scroll)
    {
        // scroll is typically positive for zoom out, negative for zoom in
        float newSize = cam.orthographicSize - (scroll * zoomSpeed);
        cam.orthographicSize = Mathf.Clamp(newSize, zoomMin, zoomMax);
    }

    private void ClampCameraX()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = fixedY;
        pos.z = fixedZ;
        transform.position = pos;
    }

    // If you want re-focus:
    public void Refocus(Transform loco)
    {
        Vector3 p = transform.position;
        p.x = loco.position.x;
        transform.position = p;
        ClampCameraX();
    }

    // If you want dynamic bounds from an event approach
    public void SetCameraBounds(float left, float right)
    {
        leftBound = left;
        rightBound = right;
        ClampCameraX();
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        currentMousePos = context.ReadValue<Vector2>();
        
        if (isDragging)
        {
            Vector2 delta = currentMousePos - lastMousePos;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0) * dragSpeed * Time.deltaTime;
            transform.position += move;
            lastMousePos = currentMousePos;
        }
    }


}
