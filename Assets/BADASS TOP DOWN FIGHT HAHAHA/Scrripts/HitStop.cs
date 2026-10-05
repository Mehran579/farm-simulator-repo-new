using System.Collections;
using UnityEngine;

// Put this on one empty GameObject in the scene. Call it from anywhere: HitStop.Do(0.05f);
public class HitStop : MonoBehaviour
{
    static HitStop instance;
    Coroutine running;

    void Awake() => instance = this;

    // If the scene reloads mid-freeze, don't leave the game stuck at timeScale 0
    void OnDestroy()
    {
        if (instance == this) instance = null;
        Time.timeScale = 1f;
    }

    public static void Do(float duration)
    {
        if (instance == null) return;

        if (instance.running != null) instance.StopCoroutine(instance.running);
        instance.running = instance.StartCoroutine(instance.Freeze(duration));
    }

    IEnumerator Freeze(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);   // realtime, because normal time is stopped
        Time.timeScale = 1f;
        running = null;
    }
}