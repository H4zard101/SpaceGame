using System.Collections;
using UnityEngine;
using UnityEngine.UI; // For CanvasGroup, but it's actually in UnityEngine

public class CreditsFader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeDuration = 2f; // How long the fade takes, in seconds—make it quick or slow, your call

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Start invisible, like a surprise attack
        StartCoroutine(FadeIn()); // Boom, show up on scene start
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        // After fade in, maybe wait a bit then fade out—add your own delay here if you want
        yield return new WaitForSeconds(5f); // Wait 5 seconds to read credits, tremendous
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }
        // Now it's gone—maybe load the main menu or quit, whatever wins big
        // Example: SceneManager.LoadScene("MainMenu");
    }
}
