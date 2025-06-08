using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;
    public float zoomSpeed = 10f;
    public float minZoom = 2f;
    public float maxZoom = 100f;

    private float yaw = 0f;
    private float pitch = 0f;

    private Vector3 targetPosition;
    private float targetDistance = 20f;

    void Start()
    {
        targetPosition = transform.position + transform.forward * targetDistance;
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        HandleRotation();
        HandleZoom();
        HandleMovement();

        UpdateCameraTransform();
    }

    void HandleMovement()
    {
        Vector3 input = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        Vector3 move = Quaternion.Euler(0, yaw, 0) * input;
        targetPosition += move * moveSpeed * Time.deltaTime;
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

            pitch = Mathf.Clamp(pitch, -89f, 89f); // Look almost fully up/down
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetDistance -= scroll * zoomSpeed;
        targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);
    }

    void UpdateCameraTransform()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -targetDistance);
        transform.position = targetPosition + offset;
        transform.rotation = rotation;
    }
}
