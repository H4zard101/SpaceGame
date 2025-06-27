using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; } // Singleton

    public List<GameObject> allUnitSelected = new List<GameObject>(); // for Multi Unit selection Needed elsewhere do not remove this
    public List<GameObject> unitSelected = new List<GameObject>(); // individual selection

    public GameObject playerMarker; // A game object that will apear underneath the ship to show that it is selected
    public LayerMask clickable; // a layer that I created so that I can add all clickable objects to it

    private Camera cam; // Main camera

    private Vector3? formationCenter = null; // Nullable Vector3 for formation center, null means no move command active
    private float spacing = 100f;

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
    }

    public void Update()
    {
        // Left-click selection handling
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        // Right-click to set formation center and move units
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (unitSelected.Count == 0) return;

                formationCenter = hit.point;
            }
        }

        // If a formation center is active, continuously update unit formation targets every frame
        if (formationCenter.HasValue && unitSelected.Count > 0)
        {
            int count = unitSelected.Count;
            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            float half = (columns - 1) / 2f;

            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int col = i % columns;

                Vector3 offset = new Vector3(
                    (col - half) * spacing,
                    0f,
                    (row - half) * spacing
                );

                Vector3 formationTarget = formationCenter.Value + Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * offset;

                unitSelected[i].GetComponent<UnitMovement>().SetFormationTarget(formationTarget);
            }
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
        if (unitSelected.Contains(unit) == false)
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
    }

    private void EnableUnitMovement(GameObject unit, bool shouldEnable)
    {
        var moveScript = unit.GetComponent<UnitMovement>();
        if (moveScript != null)
        {
            moveScript.SetSelected(shouldEnable);
        }
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisable)
    {
        unit.transform.GetChild(0).gameObject.SetActive(isVisable);// ground marker
        unit.transform.GetChild(1).gameObject.SetActive(isVisable);// health and shield bars
    }

    public void DragSelect(GameObject unit) // Do not remove this function
    {
        if (unitSelected.Contains(unit) == false) // add all objects in the box to the list of units selected.
        {
            unitSelected.Add(unit);
            TriggerSelectionIndicator(unit, true);
            EnableUnitMovement(unit, true);
        }
    }
}