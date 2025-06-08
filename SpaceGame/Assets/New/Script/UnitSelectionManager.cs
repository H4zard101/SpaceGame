using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitSelected = new List<GameObject>();
    public List<GameObject> unitSelected = new List<GameObject>();


    public GameObject playerMarker;
    public LayerMask clickable;

    private Camera cam;

    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
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
    }

    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisable)
    {
        unit.transform.GetChild(0).gameObject.SetActive(isVisable);
    }

    public void DragSelect(GameObject unit)
    {
        if(unitSelected.Contains(unit) == false)
        {
            unitSelected.Add(unit);
            TriggerSelectionIndicator(unit, true);
            EnableUnitMovement(unit, true);
        }
    }
}
