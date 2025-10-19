using UnityEngine;
using TMPro;

public class CreditsTextFade : MonoBehaviour
{
    public RectTransform bottomFadeTrigger;
    public RectTransform topFadeTrigger;

    public float fadeRange = 100f; // Distance over which fade happens

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        float textY = rectTransform.position.y;
        float bottomY = bottomFadeTrigger.position.y;
        float topY = topFadeTrigger.position.y;

        // Fade In
        if (textY < bottomY)
        {
            float t = Mathf.InverseLerp(bottomY - fadeRange, bottomY, textY);
            canvasGroup.alpha = Mathf.Clamp01(t);
        }
        // Fade Out
        else if (textY > topY)
        {
            float t = Mathf.InverseLerp(topY, topY + fadeRange, textY);
            canvasGroup.alpha = 1f - Mathf.Clamp01(t);
        }
        else
        {
            canvasGroup.alpha = 1f;
        }
    }
}
