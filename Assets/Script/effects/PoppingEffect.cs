using UnityEngine;
using System.Collections;

public class PoppingEffect : MonoBehaviour
{
    [Header("Pop Settings")]
    public Vector3 targetScale = Vector3.one;
    public float popDuration = 0.2f;
    public float overshoot = 1.15f;

    [Header("Floating Settings")]
    public float floatHeight = 0.15f;
    public float floatSpeed = 2f;

    private Vector3 startPosition;

    private void OnEnable()
    {
        startPosition = transform.position;

        transform.localScale = Vector3.zero;

        StartCoroutine(PopIn());
    }

    private IEnumerator PopIn()
    {
        float time = 0f;

        Vector3 overshootScale = targetScale * overshoot;

        while (time < popDuration)
        {
            time += Time.deltaTime;

            float t = time / popDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            transform.localScale = Vector3.Lerp(
                Vector3.zero,
                overshootScale,
                t
            );

            yield return null;
        }

        time = 0f;

        while (time < popDuration * 0.4f)
        {
            time += Time.deltaTime;

            float t = time / (popDuration * 0.4f);
            transform.localScale = Vector3.Lerp(
                overshootScale,
                targetScale,
                t
            );

            yield return null;
        }

        transform.localScale = targetScale;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = startPosition + Vector3.up * offset;
    }
}