using UnityEngine;

/// Put this on VCam_BirdsEye (the CinemachineVirtualCamera object).
/// Works whether your lens is Orthographic (recommended) or Perspective.
public class BirdsEyeControls : MonoBehaviour
{
    [Header("Pan")]
    public float panSpeed = 80f;
    public float fastPanMultiplier = 2f;        // hold Shift to pan faster
    public bool edgeScroll = true;
    public int edgeThickness = 12;              // pixels from screen edge

    [Header("Zoom (Orthographic)")]
    public float orthoZoomSpeed = 200f;
    public float orthoMinSize = 30f;
    public float orthoMaxSize = 300f;

    [Header("Zoom (Perspective: moves height)")]
    public float heightZoomSpeed = 500f;
    public float minHeight = 40f;
    public float maxHeight = 600f;

    [Header("Optional rotation (right mouse drag)")]
    public bool allowYawRotate = false;
    public float yawSpeed = 120f;

    [Header("Bounds (optional)")]
    public bool clampBounds = false;
    public Vector2 xLimits = new Vector2(-1000, 1000);
    public Vector2 zLimits = new Vector2(-1000, 1000);

    Camera mainCam;
    float yaw;  // store yaw so we can rotate around Y while keeping X=90

    void Start()
    {
        mainCam = Camera.main;
        var e = transform.eulerAngles;
        yaw = e.y;
        transform.rotation = Quaternion.Euler(90f, yaw, 0f); // keep top-down
    }

    void Update()
    {
        HandlePan();
        HandleZoom();
        HandleYaw();
        ClampIfNeeded();
    }

    void HandlePan()
    {
        Vector3 dir = Vector3.zero;

        // WASD input
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        // Fallback if you're on the new Input System and axes are unmapped
        if (Mathf.Approximately(v, 0f))
            v = (Input.GetKey(KeyCode.W) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
        if (Mathf.Approximately(h, 0f))
            h = (Input.GetKey(KeyCode.D) ? 1f : 0f) + (Input.GetKey(KeyCode.A) ? -1f : 0f);

        dir += Vector3.forward * v;
        dir += Vector3.right * h;

        // Edge scroll (optional)
        if (edgeScroll && Cursor.visible)
        {
            Vector3 m = Input.mousePosition;
            if (m.x <= edgeThickness) dir += Vector3.left;
            else if (m.x >= Screen.width - edgeThickness) dir += Vector3.right;

            if (m.y <= edgeThickness) dir += Vector3.back;
            else if (m.y >= Screen.height - edgeThickness) dir += Vector3.forward;
        }

        if (dir.sqrMagnitude > 1f) dir.Normalize();

        float speed = panSpeed * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? fastPanMultiplier : 1f);

        // ? Build planar basis from yaw (so forward isn't zero when pitched 90°)
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 planarFwd = yawRot * Vector3.forward; // (xz only)
        Vector3 planarRight = yawRot * Vector3.right;

        Vector3 move = (planarFwd * dir.z + planarRight * dir.x) * speed * Time.deltaTime;
        transform.position += move;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.0001f) return;

        // If Main Camera is orthographic, change size; else change height.
        if (mainCam != null && mainCam.orthographic)
        {
            float size = mainCam.orthographicSize;
            size -= scroll * orthoZoomSpeed * Time.deltaTime;
            size = Mathf.Clamp(size, orthoMinSize, orthoMaxSize);
            mainCam.orthographicSize = size;
        }
        else
        {
            // Move along world Y (up/down)
            float y = transform.position.y;
            y -= scroll * heightZoomSpeed * Time.deltaTime;
            y = Mathf.Clamp(y, minHeight, maxHeight);
            var p = transform.position;
            p.y = y;
            transform.position = p;
        }
    }

    void HandleYaw()
    {
        if (!allowYawRotate) return;
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * yawSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(90f, yaw, 0f);
        }
    }

    void ClampIfNeeded()
    {
        if (!clampBounds) return;
        var p = transform.position;
        p.x = Mathf.Clamp(p.x, xLimits.x, xLimits.y);
        p.z = Mathf.Clamp(p.z, zLimits.x, zLimits.y);
        transform.position = p;
    }
}
