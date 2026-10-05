using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    Color originalColor;
    Coroutine running;

    void Awake()
    {
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
        originalColor = sprite.color;
    }

    public void Flash() => Restart(FlashRoutine());

    public void Blink(float duration, float interval) => Restart(BlinkRoutine(duration, interval));

    void Restart(IEnumerator routine)
    {
        if (running != null) StopCoroutine(running);
        ResetSprite();
        running = StartCoroutine(routine);
    }

    void ResetSprite()
    {
        sprite.color = originalColor;
        sprite.enabled = true;
    }

    IEnumerator FlashRoutine()
    {
        sprite.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        ResetSprite();
    }

    IEnumerator BlinkRoutine(float duration, float interval)
    {
        float timer = 0f;
        while (timer < duration)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(interval);
            timer += interval;
        }
        ResetSprite();
    }
}