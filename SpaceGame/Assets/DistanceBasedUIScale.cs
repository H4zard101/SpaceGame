using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBasedUIScale : MonoBehaviour
{
    public float scaleFactor = 1.0f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;
    public float minDistance = 2f;
    public float maxDistance = 20f;

    private Camera mainCamera;
    private Vector3 initialScale;

    void Start()
    {
        mainCamera = Camera.main;
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float scaleMultiplier = Mathf.Lerp(minScale, maxScale, t) * scaleFactor;

        transform.localScale = initialScale * scaleMultiplier;
    }
}
