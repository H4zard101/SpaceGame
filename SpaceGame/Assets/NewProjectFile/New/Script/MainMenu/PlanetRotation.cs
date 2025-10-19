using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public float rotationSpeed;
    public GameObject pivotPoint;
    void Start()
    {
        
    }

    void Update()
    {
        transform.RotateAround(pivotPoint.transform.position, new Vector3(1, 1, 0), rotationSpeed * Time.deltaTime);
    }
}
