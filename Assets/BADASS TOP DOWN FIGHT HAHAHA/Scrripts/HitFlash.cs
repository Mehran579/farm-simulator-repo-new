using System.Collections;
using UnityEngine;

// Put this on the enemy prefab and on the player (same object as the Enemy / playerhealth script).
public class HitFlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;      // leave empty to auto-find on this object or a child
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] float flashDuration = 0.1f;

    Color originalColor;
    Coroutine running;

    void Awake()
    {
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
        originalColor = sprite.color;
    }

    // One quick colour flash (used by enemies)
    public void Flash() => Restart(FlashRoutine());

    // Sprite blinks on/off for a while (used by the player)
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