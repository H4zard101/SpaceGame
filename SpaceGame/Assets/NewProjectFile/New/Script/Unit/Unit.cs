using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    
    void Start()
    {
        UnitSelectionManager.Instance.allUnitSelected.Add(this.gameObject);
    }
    private void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitSelected.Remove(this.gameObject);
    }
}
