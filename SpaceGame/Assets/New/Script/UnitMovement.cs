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

            if (Input.GetMouseButtonDown(1))
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

        // Move towards formation target if assigned
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
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            if (hasFormationTarget)
            {
                hasFormationTarget = false; // reached formation target
            }
            else if (hasMoveCommand)
            {
                hasMoveCommand = false; // reached manual move target
            }
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
            //hasFormationTarget = false;
            //hasMoveCommand = false;
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

    // NEW method for continuous formation movement
    public void SetFormationTarget(Vector3 target)
    {
        formationTarget = target;
        hasFormationTarget = true;
        isSettingHeight = false;
        hasMoveCommand = false;
        lineRenderer.enabled = false;
    }
}