using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if(_mainCamera == null)
        {
            return;
        }
        transform.LookAt(_mainCamera.transform);
        transform.Rotate(0, 180, 0);
    }
}
