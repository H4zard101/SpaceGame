using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private LineRenderer lineRenderer;     // Draws preview lines
    private Camera cam;                    // Main camera reference

    private Vector3 groundClickPoint;      // XZ point where player initially right-clicks
    private Vector3 finalDestination;      // Final 3D point (for height setting)
    private float moveSpeed = 10f;         // Movement speed of the unit

    private bool isSelected = false;       // Is this unit currently selected
    private bool isSettingHeight = false;  // Is the player dragging to set height
    private bool hasMoveCommand = false;   // Has a movement command been issued

    public LayerMask clickable;

    private Vector3 formationTarget;
    private bool hasFormationTarget = false;

    [Header("Separation")]
    public float separationRadius = 8f;
    public float separationStrength = 12f;
    public LayerMask shipMask;

    [Header("Arrival")]
    public float slowRadius = 4f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        cam = Camera.main;

        lineRenderer.positionCount = 4;
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
    }

    void Update()
    {
        if (isSelected)
        {
            DrawXZPreviewToMouse();

            // IGNORE RMB if formation was just issued this frame
            if (Input.GetMouseButtonDown(1) && UnitSelectionManager.Instance.rightClickIssuedFrame != Time.frameCount)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

                if (plane.Raycast(ray, out float distance))
                {
                    groundClickPoint = ray.GetPoint(distance);
                    isSettingHeight = true;
                }
            }

            if (isSettingHeight && Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPreview();
            }

            if (isSettingHeight)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Vector3 heightPoint = groundClickPoint;

                Plane heightPlane = new Plane(Vector3.forward, groundClickPoint);
                if (heightPlane.Raycast(ray, out float hDist))
                {
                    Vector3 heightMouse = ray.GetPoint(hDist);
                    heightPoint.y = heightMouse.y;
                }

                finalDestination = heightPoint;

                DrawMovementTriangle(finalDestination);

                if (Input.GetMouseButtonUp(1))
                {
                    isSettingHeight = false;
                    lineRenderer.enabled = false;
                    hasMoveCommand = true;
                    hasFormationTarget = false;  // Cancel formation target when manually setting height move
                }
            }
        }

        if (hasFormationTarget)
        {
            MoveTowardsTarget(formationTarget);
        }
        else if (hasMoveCommand && !isSettingHeight)
        {
            MoveTowardsTarget(finalDestination);
        }
    }

    void MoveTowardsTarget(Vector3 target)
    {
        Vector3 pos = transform.position;

        // Desired direction
        Vector3 toTarget = target - pos;
        float dist = toTarget.magnitude;
        Vector3 desiredDir = dist > 0.0001f ? (toTarget / dist) : Vector3.zero;

        // Separation (XZ only)
        Vector3 sep = Vector3.zero;
        if (separationRadius > 0.01f)
        {
            Collider[] near = Physics.OverlapSphere(pos, separationRadius, shipMask, QueryTriggerInteraction.Ignore);
            foreach (var col in near)
            {
                if (col.transform == transform) continue;
                Vector3 away = pos - col.transform.position;
                away.y = 0f;
                float d = away.magnitude;
                if (d > 0.0001f)
                {
                    sep += away.normalized / (d * d);
                }
            }
            if (sep.sqrMagnitude > 0.0001f)
                sep = sep.normalized * separationStrength;
        }

        // Combine with desired direction (XZ only)
        Vector3 steerXZ = new Vector3(desiredDir.x, 0f, desiredDir.z) + new Vector3(sep.x, 0f, sep.z);
        if (steerXZ.sqrMagnitude > 0.0001f) steerXZ.Normalize();

        float speed = moveSpeed;
        if (dist < slowRadius)
            speed = Mathf.Lerp(0.25f * moveSpeed, moveSpeed, Mathf.InverseLerp(0f, slowRadius, dist));

        float step = speed * Time.deltaTime;

        Vector3 xzNow = new Vector3(pos.x, 0f, pos.z);
        Vector3 xzTarget = new Vector3(target.x, 0f, target.z);
        Vector3 xzNext = Vector3.MoveTowards(xzNow, xzTarget, step);

        float yNext = Mathf.MoveTowards(pos.y, target.y, step);

        Vector3 next = new Vector3(xzNext.x, yNext, xzNext.z);

        // Rotate toward movement
        Vector3 vel = next - pos;
        Vector3 velXZ = new Vector3(vel.x, 0f, vel.z);
        if (velXZ.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(velXZ.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
        }

        transform.position = next;

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            if (hasFormationTarget) hasFormationTarget = false;
            else if (hasMoveCommand) hasMoveCommand = false;
        }
    }

    void DrawMovementTriangle(Vector3 heightPoint)
    {
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, groundClickPoint);
        lineRenderer.SetPosition(2, heightPoint);
        lineRenderer.SetPosition(3, transform.position);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (!selected)
        {
            isSettingHeight = false;
            lineRenderer.enabled = false;
        }
    }

    void DrawXZPreviewToMouse()
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 mousePosOnXZ = ray.GetPoint(distance);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, mousePosOnXZ);
        }
    }

    void CancelPreview()
    {
        isSettingHeight = false;
        hasMoveCommand = false;
        lineRenderer.enabled = false;
    }

    public void SetDestination(Vector3 destination)
    {
        finalDestination = destination;
        hasMoveCommand = true;
        isSettingHeight = false;
        lineRenderer.enabled = false;
        hasFormationTarget = false;
    }

    public void SetFormationTarget(Vector3 target)
    {
        formationTarget = target;
        hasFormationTarget = true;
        isSettingHeight = false;
        hasMoveCommand = false;
        lineRenderer.enabled = false;
    }
}
