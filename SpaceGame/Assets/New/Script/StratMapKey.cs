using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StratMapKey : MonoBehaviour
{
    public GameObject stratMap;

    private Vector3 initPos;
    public Vector3 pressedOffset = new Vector3(0, -2, 0); // Move down by 2 units
    private Vector3 pressedPos;

    public float speed = 10.0f;

    private bool isAnimating = false;

    public void Start()
    {
        stratMap = GameObject.Find("MapKey");

        if (stratMap == null)
        {
            Debug.LogError("MapKey object not found in scene.");
            return;
        }

        initPos = stratMap.transform.position;
        pressedPos = initPos + pressedOffset;
    }

    public void OnButtonPressed()
    {
        if (!isAnimating)
        {
            StartCoroutine(MoveStratMap());
        }
    }

    private IEnumerator MoveStratMap()
    {
        isAnimating = true;

        // Move to pressedPos
        yield return StartCoroutine(MoveObject(stratMap, stratMap.transform.position, pressedPos, speed));

        // Wait for a short delay
        yield return new WaitForSeconds(0.5f);

        // Move back to initPos
        yield return StartCoroutine(MoveObject(stratMap, stratMap.transform.position, initPos, speed));

        isAnimating = false;
    }

    private IEnumerator MoveObject(GameObject obj, Vector3 startPos, Vector3 endPos, float moveSpeed)
    {
        float elapsedTime = 0f;
        float duration = Vector3.Distance(startPos, endPos) / moveSpeed;

        while (elapsedTime < duration)
        {
            obj.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = endPos;
    }
}
