using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; } // Singleton

    public List<GameObject> allUnitSelected = new List<GameObject>(); // for Multi Unit selection Needed elsewhere do not remove this
    public List<GameObject> unitSelected = new List<GameObject>(); // individual selection

    public GameObject playerMarker; // A game object that will apear underneath the ship to show that it is selected
    public LayerMask clickable; // a layer that I created so that I can add all clickable objects to it

    private Camera cam; // Main camera

    private Vector3? formationCenter = null; // Nullable Vector3 for formation center, null means no move command active

    // NOTE: 'spacing' kept (original var). Acts as extra buffer on top of size-based spacing.
    private float spacing = 6f;

    private Vector3 MousePosition;
    public Transform hoverHitTransform;

    private GameObject lastHoveredUnit = null;

    // Dictionary to store control groups (keys: 1–5, values: lists of units)
    private Dictionary<int, List<GameObject>> unitGroups = new Dictionary<int, List<GameObject>>();

    // Tells UnitMovement to ignore the RMB this frame (prevents canceling formation)
    public int rightClickIssuedFrame;

    // RMB hold-to-adjust height for formations (movement only on RMB UP)
    private bool isFormationDragging = false;
    private Vector3 formationGroundClickPoint; // XZ is locked here; Y will change during drag

    // ===== Size-aware spacing controls =====
    [Header("Size-Aware Spacing")]
    [Tooltip("Multiplier applied to each ship's footprint to create safe gaps.")]
    public float sizePadding = 1.15f; // 15% padding
    [Tooltip("Minimum spacing floor (world units) after padding.")]
    public float minSpacing = 2f;
    [Tooltip("Maximum spacing cap to prevent extreme gaps (world units).")]
    public float maxSpacing = 25f;

    // ===== Formation selection =====
    public enum FormationShape { Square, Wedge, Line, Column }
    [Header("Formation")]
    public FormationShape formationShape = FormationShape.Square;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        cam = Camera.main;

        // Initialise empty lists for groups 1–5
        for (int i = 1; i <= 5; i++)
        {
            unitGroups[i] = new List<GameObject>();
        }
    }

    public void Update()
    {
        // ======== Quick formation switching hotkeys (optional) ========
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            int idx = (int)formationShape - 1;
            if (idx < 0) idx = Enum.GetValues(typeof(FormationShape)).Length - 1;
            formationShape = (FormationShape)idx;
            Debug.Log($"Formation: {formationShape}");
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            int idx = (int)formationShape + 1;
            if (idx >= Enum.GetValues(typeof(FormationShape)).Length) idx = 0;
            formationShape = (FormationShape)idx;
            Debug.Log($"Formation: {formationShape}");
        }

        // ======== Hover Indicator Logic ========
        MousePosition = Input.mousePosition;
        Ray mouseRay = cam.ScreenPointToRay(MousePosition);

        if (Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, clickable))
        {
            GameObject hoveredUnit = hit.collider.gameObject;

            if (hoveredUnit != lastHoveredUnit)
            {
                if (lastHoveredUnit != null && !unitSelected.Contains(lastHoveredUnit))
                {
                    // Hide previous hover indicator if it’s not selected
                    TriggerSelectionIndicator(lastHoveredUnit, false);
                }

                if (!unitSelected.Contains(hoveredUnit))
                {
                    // Show hover indicator only if not already selected
                    TriggerSelectionIndicator(hoveredUnit, true);
                }

                lastHoveredUnit = hoveredUnit;
            }
        }
        else
        {
            // If nothing hovered, remove previous hover indicator if applicable
            if (lastHoveredUnit != null && !unitSelected.Contains(lastHoveredUnit))
            {
                TriggerSelectionIndicator(lastHoveredUnit, false);
            }
            lastHoveredUnit = null;
        }

        // ======== Selection Handling ========
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        // ======== RMB DOWN: lock XZ center on plane at avg fleet Y (NO MOVEMENT YET) ========
        if (Input.GetMouseButtonDown(1))
        {
            if (unitSelected.Count > 0)
            {
                // Average Y of selected ships
                float avgY = 0f;
                foreach (var go in unitSelected) avgY += go.transform.position.y;
                avgY /= Mathf.Max(1, unitSelected.Count);

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Plane planeXZ = new Plane(Vector3.up, new Vector3(0f, avgY, 0f));
                if (planeXZ.Raycast(ray, out float t))
                {
                    Vector3 centerXZ = ray.GetPoint(t);
                    formationGroundClickPoint = centerXZ; // lock XZ here
                    formationCenter = centerXZ;           // start height from here
                    isFormationDragging = true;

                    // Prevent UnitMovement from treating this same-frame RMB as a manual move
                    rightClickIssuedFrame = Time.frameCount;
                }
            }
        }

        // ======== RMB HELD: adjust ONLY the height (no movement yet) ========
        if (isFormationDragging && Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Mirror UnitMovement: plane with forward normal through the click point
            Plane heightPlane = new Plane(Vector3.forward, formationGroundClickPoint);
            Vector3 adjustedCenter = formationGroundClickPoint;

            if (heightPlane.Raycast(ray, out float hDist))
            {
                Vector3 heightMouse = ray.GetPoint(hDist);
                adjustedCenter.y = heightMouse.y; // only Y changes while dragging
            }

            formationCenter = adjustedCenter;
        }

        // ======== RMB UP: finally issue the formation (movement happens now) ========
        if (isFormationDragging && Input.GetMouseButtonUp(1))
        {
            isFormationDragging = false;

            if (formationCenter.HasValue)
            {
                IssueFormationTo(formationCenter.Value);
            }

            formationCenter = null;
        }

        // ======== Control Group Hotkeys ========
        HandleGroupHotkeys();
    }

    // Issues formation slots to currently selected units around 'center'
    private void IssueFormationTo(Vector3 center)
    {
        if (unitSelected.Count == 0) return;

        // Build orientation frame (camera-aligned on XZ)
        Vector3 camFwd = cam.transform.forward;
        camFwd.y = 0f;
        if (camFwd.sqrMagnitude < 0.001f) camFwd = Vector3.forward;
        camFwd.Normalize();
        Vector3 right = Quaternion.Euler(0f, 90f, 0f) * camFwd;

        // Measure footprints
        int count = unitSelected.Count;
        float[] widths = new float[count];
        float[] depths = new float[count];

        for (int i = 0; i < count; i++)
        {
            GetFootprintXZ(unitSelected[i], out float w, out float d);
            w = Mathf.Clamp(w * sizePadding, minSpacing, maxSpacing) + spacing;
            d = Mathf.Clamp(d * sizePadding, minSpacing, maxSpacing) + spacing;
            widths[i] = w;
            depths[i] = d;
        }

        // 1) Build slots for the chosen shape (just positions; not yet assigned to ships)
        List<Vector3> slots;
        switch (formationShape)
        {
            case FormationShape.Wedge:
                slots = BuildSlotsWedge(center, right, camFwd, widths, depths);
                break;
            case FormationShape.Line:
                slots = BuildSlotsLine(center, right, camFwd, widths, depths);
                break;
            case FormationShape.Column:
                slots = BuildSlotsColumn(center, right, camFwd, widths, depths);
                break;
            case FormationShape.Square:
            default:
                slots = BuildSlotsSquare(center, right, camFwd, widths, depths);
                break;
        }

        // 2) Make ship 0 the head: pick the most "front-center" slot for it.
        int headSlot = GetFrontCenterSlotIndex(slots, center, right, camFwd);

        // 3) Assign targets: ship 0 -> head slot, others -> remaining slots in order
        bool[] used = new bool[slots.Count];
        used[headSlot] = true;

        // head
        var mv0 = unitSelected[0].GetComponent<UnitMovement>();
        if (mv0 != null) mv0.SetFormationTarget(slots[headSlot]);

        // everyone else
        int s = 0;
        for (int i = 1; i < unitSelected.Count; i++)
        {
            while (s < slots.Count && used[s]) s++;
            if (s >= slots.Count) break;

            var mv = unitSelected[i].GetComponent<UnitMovement>();
            if (mv != null) mv.SetFormationTarget(slots[s]);

            used[s] = true;
            s++;
        }
    }

    // ---------- Build slots for each shape (size-aware) ----------

    // Grid (row-centered). Rows trail toward -fwd, row width is size-aware.
    private List<Vector3> BuildSlotsSquare(Vector3 center, Vector3 right, Vector3 fwd, float[] widths, float[] depths)
    {
        int count = widths.Length;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt(count / (float)columns);

        var slots = new List<Vector3>(count);
        float cumulativeZ = 0f;

        for (int r = 0; r < rows; r++)
        {
            int startIdx = r * columns;
            int endIdx = Mathf.Min(startIdx + columns, count);

            float rowWidth = 0f;
            float rowMaxDepth = 0f;
            for (int i = startIdx; i < endIdx; i++)
            {
                rowWidth += widths[i];
                rowMaxDepth = Mathf.Max(rowMaxDepth, depths[i]);
            }

            float xCursor = -rowWidth * 0.5f;

            for (int i = startIdx; i < endIdx; i++)
            {
                float w = widths[i];

                float xLocal = xCursor + w * 0.5f;
                float zLocal = -cumulativeZ;

                slots.Add(center + right * xLocal + fwd * zLocal);
                xCursor += w;
            }

            cumulativeZ += rowMaxDepth;
        }

        return slots;
    }

    // Wedge: 1,2,4,6... per row, centered rows, trailing toward -fwd
    private List<Vector3> BuildSlotsWedge(Vector3 center, Vector3 right, Vector3 fwd, float[] widths, float[] depths)
    {
        int count = widths.Length;
        var slots = new List<Vector3>(count);

        List<int> rowCounts = new List<int>();
        int remaining = count;
        int row = 0;
        while (remaining > 0)
        {
            int want = row == 0 ? 1 : (row * 2);
            int take = Mathf.Min(want, remaining);
            rowCounts.Add(take);
            remaining -= take;
            row++;
        }

        int idx = 0;
        float cumulativeZ = 0f;

        foreach (int take in rowCounts)
        {
            int startIdx = idx;
            int endIdx = idx + take;

            float rowWidth = 0f;
            float rowMaxDepth = 0f;
            for (int i = startIdx; i < endIdx; i++)
            {
                rowWidth += widths[i];
                rowMaxDepth = Mathf.Max(rowMaxDepth, depths[i]);
            }

            float xCursor = -rowWidth * 0.5f;

            for (int i = startIdx; i < endIdx; i++)
            {
                float w = widths[i];

                float xLocal = xCursor + w * 0.5f;
                float zLocal = -cumulativeZ;

                slots.Add(center + right * xLocal + fwd * zLocal);
                xCursor += w;
            }

            cumulativeZ += rowMaxDepth;
            idx = endIdx;
        }

        return slots;
    }

    // Single centered row
    private List<Vector3> BuildSlotsLine(Vector3 center, Vector3 right, Vector3 fwd, float[] widths, float[] depths)
    {
        int count = widths.Length;
        var slots = new List<Vector3>(count);

        float totalWidth = 0f;
        for (int i = 0; i < count; i++) totalWidth += widths[i];

        float xCursor = -totalWidth * 0.5f;
        for (int i = 0; i < count; i++)
        {
            float w = widths[i];
            float xLocal = xCursor + w * 0.5f;

            slots.Add(center + right * xLocal);
            xCursor += w;
        }
        return slots;
    }

    // Single column trailing backward (size-aware by depth)
    private List<Vector3> BuildSlotsColumn(Vector3 center, Vector3 right, Vector3 fwd, float[] widths, float[] depths)
    {
        int count = widths.Length;
        var slots = new List<Vector3>(count);

        float cumulativeZ = 0f;
        for (int i = 0; i < count; i++)
        {
            float d = depths[i];
            float zLocal = -cumulativeZ;

            slots.Add(center + fwd * zLocal);
            cumulativeZ += d;
        }
        return slots;
    }

    // ---------- Choose the "front-center" slot for the head ----------
    // We prefer the slot with the largest Z (closest to the front),
    // breaking ties by |X| (closest to center).
    private int GetFrontCenterSlotIndex(List<Vector3> slots, Vector3 center, Vector3 right, Vector3 fwd)
    {
        int best = 0;
        float bestFront = float.NegativeInfinity;
        float bestCenterAbs = float.PositiveInfinity;

        foreach (int i in System.Linq.Enumerable.Range(0, slots.Count))
        {
            Vector3 local = slots[i] - center;
            // project onto our frame
            float x = Vector3.Dot(local, right.normalized);
            float z = Vector3.Dot(local, fwd.normalized);

            // higher z = more front; lower |x| = more center
            if (z > bestFront + 1e-4f || (Mathf.Abs(z - bestFront) <= 1e-4f && Mathf.Abs(x) < bestCenterAbs))
            {
                best = i;
                bestFront = z;
                bestCenterAbs = Mathf.Abs(x);
            }
        }
        return best;
    }

    // Get a ship's approximate footprint on X/Z (width & depth) from Collider or Renderer
    private void GetFootprintXZ(GameObject go, out float width, out float depth)
    {
        width = 6f; depth = 6f; // defaults

        // Prefer Collider bounds (more physical)
        Collider col = go.GetComponent<Collider>();
        if (col != null)
        {
            Bounds b = col.bounds;
            width = Mathf.Max(0.01f, b.size.x);
            depth = Mathf.Max(0.01f, b.size.z);
            return;
        }

        // Fallback to Renderer bounds
        Renderer r = go.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            Bounds b = r.bounds;
            width = Mathf.Max(0.01f, b.size.x);
            depth = Mathf.Max(0.01f, b.size.z);
        }
    }

    void HandleLeftClick()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                MultiSelect(hit.collider.gameObject);
            }
            else
            {
                SelectByClicking(hit.collider.gameObject);
            }
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DeselectAll();
            }
        }
    }

    private void MultiSelect(GameObject unit)
    {
        if (!unitSelected.Contains(unit))
        {
            unitSelected.Add(unit);
            EnableUnitMovement(unit, true);
            TriggerSelectionIndicator(unit, true);
        }
        else
        {
            EnableUnitMovement(unit, false);
            unitSelected.Remove(unit);
            TriggerSelectionIndicator(unit, false);
        }
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();
        unitSelected.Add(unit);
        TriggerSelectionIndicator(unit, true);
        EnableUnitMovement(unit, true);
    }

    public void DeselectAll()
    {
        foreach (var unit in unitSelected)
        {
            EnableUnitMovement(unit, false);
            TriggerSelectionIndicator(unit, false);
        }
        unitSelected.Clear();
        formationCenter = null; // Reset formation center when deselecting
        isFormationDragging = false;
    }

    private void EnableUnitMovement(GameObject unit, bool shouldEnable)
    {
        var moveScript = unit.GetComponent<UnitMovement>();
        if (moveScript != null)
        {
            moveScript.SetSelected(shouldEnable);
        }
    }

    public void TriggerSelectionIndicator(GameObject unit, bool isVisable)
    {
        unit.transform.GetChild(0).gameObject.SetActive(isVisable); // ground marker
        unit.transform.GetChild(1).gameObject.SetActive(isVisable); // health and shield bars
    }

    public void DragSelect(GameObject unit) // Do not remove this function
    {
        if (!unitSelected.Contains(unit))
        {
            unitSelected.Add(unit);
            TriggerSelectionIndicator(unit, true);
            EnableUnitMovement(unit, true);
        }
    }

    // ========================
    // Control Group Logic
    // ========================
    private void HandleGroupHotkeys()
    {
        for (int i = 1; i <= 5; i++)
        {
            // Assign selected units to group (Alt + number)
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                // Remove from all groups first
                foreach (var group in unitGroups.Values)
                {
                    group.RemoveAll(u => unitSelected.Contains(u));
                }

                // Now assign to the chosen group
                unitGroups[i].Clear();
                unitGroups[i].AddRange(unitSelected);

                Debug.Log($"Assigned {unitGroups[i].Count} units to group {i} (removed from other groups)");
            }
            // Add selected units to existing group (Shift + number)
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                // Remove from all groups first
                foreach (var group in unitGroups.Values)
                {
                    group.RemoveAll(u => unitSelected.Contains(u));
                }

                // Add to the chosen group
                foreach (var unit in unitSelected)
                {
                    if (!unitGroups[i].Contains(unit))
                        unitGroups[i].Add(unit);
                }

                Debug.Log($"Added to group {i}. Group now has {unitGroups[i].Count} units (removed from other groups)");
            }
            // Select units in group (number key only)
            else if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                if (unitGroups[i].Count > 0)
                {
                    DeselectAll();
                    foreach (var unit in unitGroups[i])
                    {
                        if (unit != null) // Skip destroyed units
                        {
                            unitSelected.Add(unit);
                            TriggerSelectionIndicator(unit, true);
                            EnableUnitMovement(unit, true);
                        }
                    }
                    Debug.Log($"Selected group {i} with {unitSelected.Count} units.");
                }
            }
        }
    }
}
