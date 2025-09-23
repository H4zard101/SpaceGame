// FormationCommander.cs
using System.Collections.Generic;
using UnityEngine;

public class FormationCommander : MonoBehaviour
{
    [Header("Wire these from your selection system")]
    public List<UnitMovement> selectedShips = new();

    [Header("Formation")]
    public float spacing = 6f;
    public string shape = "grid"; // "grid", "line", "wedge"
    public int gridColumns = 5;

    public void IssueMove(Vector3 targetWorldPos, Vector3 forwardHint)
    {
        if (selectedShips.Count == 0) return;

        Vector3 fwd = forwardHint.sqrMagnitude > 0.001f
            ? forwardHint
            : (targetWorldPos - AveragePos()).normalized;

        var slots = FormationManager.GetSlots(
            selectedShips.Count,
            targetWorldPos,
            fwd,
            spacing,
            shape,
            gridColumns
        );

        // Stable: selection order -> slot order
        for (int i = 0; i < selectedShips.Count; i++)
        {
            selectedShips[i].SetFormationTarget(slots[Mathf.Min(i, slots.Count - 1)]);
        }
    }

    private Vector3 AveragePos()
    {
        Vector3 sum = Vector3.zero;
        foreach (var s in selectedShips) sum += s.transform.position;
        return sum / Mathf.Max(1, selectedShips.Count);
    }
}
