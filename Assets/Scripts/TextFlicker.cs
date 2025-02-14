using System.Collections;
using UnityEngine;
using TMPro;

public class TextFlicker : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float fadeDuration = 1.5f; // Time for one full fade-in/out cycle

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FadeText());
    }

    IEnumerator FadeText()
    {
        while (true)
        {
            // Fade in
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

            // Hold fully visible for a moment
            yield return new WaitForSeconds(0.5f);

            // Fade out
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

            // Hold fully invisible for a moment
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = text.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            text.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // Ensure the final alpha is set
        text.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}

