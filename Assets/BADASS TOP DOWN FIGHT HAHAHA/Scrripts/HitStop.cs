using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    static HitStop instance;
    Coroutine running;

    void Awake() => instance = this;

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
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        running = null;
    }
}