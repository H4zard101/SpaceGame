using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FormationManager
{

    public static List<Vector3> GetSlots(int count, Vector3 center, Vector4 forward, float spacing = 6f, string shape = "grid", int gridCols = 5)
    {
        var slots = new List<Vector3>(count);

        var fwd = forward.sqrMagnitude < 1e-4f ? Vector3.forward : Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
        var right = Vector3.Cross(Vector3.up, fwd).normalized;

        switch (shape)
        {
            case "line":
                for (int i = 0; i < count; i++)
                {
                    float off = (i - (count - 1) * 0.5f) * spacing;
                    slots.Add(center + right * off);
                }
                break;

            case "wedge":
                int row = 0, placed = 0;
                while (placed < count)
                {
                    int inRow = Mathf.Min(placed == 0 ? 1 : 2 * row, count - placed);
                    float half = (inRow - 1) * 0.5f;
                    for (int i = 0; i < inRow; i++)
                    {
                        float x = (i - half) * spacing;
                        float z = -row * spacing;
                        slots.Add(center + right * x + fwd * z);
                        placed++;
                        if (placed >= count) break;
                    }
                    row++;
                }
                break;

            case "grid":
            default:
                int cols = Mathf.Max(1, gridCols);
                int rows = Mathf.CeilToInt(count / (float)cols);
                int idx = 0;
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (idx >= count) break;
                        float x = (c - (cols - 1) * 0.5f) * spacing;
                        float z = -(r) * spacing;
                        slots.Add(center + right * x + fwd * z);
                        idx++;
                    }
                }
                break;
        }
        return slots;
    }
}

