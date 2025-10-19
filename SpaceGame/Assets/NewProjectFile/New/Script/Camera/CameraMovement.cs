using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float verticalSpeed = 20f;
    public float rotationSpeed = 3f;
    public float zoomSpeed = 50f;

    public float minZoom = 2f;
    public float maxZoom = 200f;

    public float focusMoveSpeed = 10f;    // how fast camera moves to target
    public float focusRotateSpeed = 5f;   // how fast camera rotates to target
    public float focusDistance = 10f;     // how far from ship to stop

    private float yaw;
    private float pitch;
    private Camera cam;

    private bool isFocusing = false;
    private Vector3 focusTargetPos;
    private Quaternion focusTargetRot;

    void Start()
    {
        cam = Camera.main;
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            TryFocusOnShip();

        if (isFocusing)
            HandleFocus();
        else
        {
            HandleRotation();
            HandleMovement();
            HandleZoom();
        }
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        move += transform.forward * Input.GetAxis("Vertical") * moveSpeed;
        move += transform.right * Input.GetAxis("Horizontal") * moveSpeed;

        if (Input.GetKey(KeyCode.Q)) move += Vector3.down * verticalSpeed;
        if (Input.GetKey(KeyCode.E)) move += Vector3.up * verticalSpeed;

        transform.position += move * Time.deltaTime;
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(2))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            Vector3 zoom = transform.forward * scroll * zoomSpeed;
            Vector3 newPos = transform.position + zoom;

            float dist = Vector3.Distance(newPos, transform.position);
            if (dist >= minZoom && dist <= maxZoom)
                transform.position = newPos;
        }
    }

    void TryFocusOnShip()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); // ray from mouse
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Ship")) // make sure your ships have the "Ship" tag
            {
                Vector3 directionToShip = (hit.point - transform.position).normalized;
                focusTargetPos = hit.point - directionToShip * focusDistance;
                focusTargetRot = Quaternion.LookRotation(hit.point - focusTargetPos);

                isFocusing = true;
            }
        }
    }

    void HandleFocus()
    {
        transform.position = Vector3.Lerp(transform.position, focusTargetPos, Time.deltaTime * focusMoveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, focusTargetRot, Time.deltaTime * focusRotateSpeed);

        if (Vector3.Distance(transform.position, focusTargetPos) < 0.1f)
        {
            isFocusing = false; // focus complete
        }
    }
}
