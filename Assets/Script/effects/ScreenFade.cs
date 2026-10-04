using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
    }

    public IEnumerator FadeToBlack()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            Color c = fadeImage.color;
            c.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadeImage.color = c;

            yield return null;
        }
    }

    public IEnumerator FadeFromBlack()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            Color c = fadeImage.color;
            c.a = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadeImage.color = c;

            yield return null;
        }
    }
}