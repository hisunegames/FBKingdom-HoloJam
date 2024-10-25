using UnityEngine;
using TMPro;
using System.Collections;

public class FadeOutText : MonoBehaviour
{
    public float fadeDuration = 1.0f; // Duration of the fade-out in seconds
    private TextMeshProUGUI textMeshPro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    // Coroutine to fade out the text and destroy the game object
    private IEnumerator FadeOutAndDestroy()
    {
        float elapsedTime = 0f;
        Color originalColor = textMeshPro.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
