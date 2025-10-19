using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuTopUI : MonoBehaviour
{
    public GameObject TopUIElement;
    public GameObject PauseMenu;

    private Vector3 initPos;
    public Vector3 pressedOffset = new Vector3(0, -2, 0); // Move down by 2 units
    private Vector3 pressedPos;

    public float speed = 10.0f;

    private bool isAnimating = false;

    public void Start()
    {
        TopUIElement = GameObject.Find("TopUIElement");
        PauseMenu.SetActive(false);

        if (TopUIElement == null)
        {
            Debug.LogError("TopUIElement object not found in scene.");
            return;
        }

        initPos = TopUIElement.transform.position;
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
        yield return StartCoroutine(MoveObject(TopUIElement, TopUIElement.transform.position, pressedPos, speed));

        // Wait for a short delay
        yield return new WaitForSeconds(0.5f);

        // Move back to initPos
        yield return StartCoroutine(MoveObject(TopUIElement, TopUIElement.transform.position, initPos, speed));

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

    public void BringUpMenu()
    {
        PauseMenu.SetActive(true);
    }
}
