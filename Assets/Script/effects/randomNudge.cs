//using System;
using System.Collections;
using UnityEngine;

public class RandomNudge : MonoBehaviour
{
    public float nudgeDistance = 0.5f;
    public float nudgeDuration = 0.1f;
    public float holdDuration = 0.2f;
    public float returnDuration = 0.4f;
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 1.0f;

    public bool is2D = true;

    public Vector3 startPosition;

    private Coroutine nudgeCoroutine;

    private void Start()
    {
        startPosition = transform.localPosition;
        StartNudge();
    }

    private void StartNudge()
    {
        nudgeCoroutine = StartCoroutine(NudgeRoutine());
    }

    private void StopNudge()
    {
        if (nudgeCoroutine != null)
        {
            StopCoroutine(nudgeCoroutine);
            nudgeCoroutine = null;
        }
    }

    private IEnumerator NudgeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minWaitTime, maxWaitTime)
            );

            Vector3 randomDirection = is2D
                ? (Vector3)Random.insideUnitCircle.normalized
                : Random.onUnitSphere;

            Vector3 targetPosition =
                startPosition + randomDirection * nudgeDistance;

            float elapsed = 0f;

            while (elapsed < nudgeDuration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / nudgeDuration;

                transform.localPosition = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

                yield return null;
            }

            if (holdDuration > 0f)
            {
                yield return new WaitForSeconds(holdDuration);
            }

            elapsed = 0f;

            while (elapsed < returnDuration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.SmoothStep(
                    0f,
                    1f,
                    elapsed / returnDuration
                );

                transform.localPosition = Vector3.Lerp(
                    targetPosition,
                    startPosition,
                    t
                );

                yield return null;
            }

            transform.localPosition = startPosition;
        }
    }

    public void ResetNudgePosition()
    {
        StopNudge();

        startPosition = transform.localPosition;

        StartNudge();
    }

    private void OnDisable()
    {
        StopNudge();
    }
}
