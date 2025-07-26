using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private Transform creditsJobTextParent; 
    [SerializeField] private Transform namesToJobsParent; 
    [SerializeField] private float scrollSpeed = 50f;

    [SerializeField] private List<TMP_Text> creditsText = new List<TMP_Text>();


    public void Start()
    {
        if (creditsJobTextParent != null)
        {
            creditsText.AddRange(creditsJobTextParent.GetComponentsInChildren<TextMeshProUGUI>(true));
        }

        if (namesToJobsParent != null)
        {
            creditsText.AddRange(namesToJobsParent.GetComponentsInChildren<TextMeshProUGUI>(true));
        }

        Debug.Log($"Found {creditsText.Count} text objects.");
    }
    private void Update()
    {
        // Move each text object up the screen
        foreach (var text in creditsText)
        {
            text.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
        }
    }
}
