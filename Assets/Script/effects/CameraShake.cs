using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static void TriggerShake(float duration, float magnitude)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("no main cam foind");
            return;
        }

        CameraShake shaker = mainCam.GetComponent<CameraShake>();
        if (shaker == null)
        {
            shaker = mainCam.gameObject.AddComponent<CameraShake>();
        }

        shaker.StartShake(mainCam, duration, magnitude);
    }

    private Coroutine shakeCoroutine;
    private Vector3 originalPosition;
    private bool isShaking = false;

    public void StartShake(Camera targetCam, float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine(targetCam, duration, magnitude));
    }

    private IEnumerator ShakeRoutine(Camera targetCam, float duration, float magnitude)
    {
        Transform camTransform = targetCam.transform;

        if (!isShaking)
        {
            originalPosition = camTransform.localPosition;
            isShaking = true;
        }

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            if (targetCam == null || !targetCam.gameObject.activeInHierarchy)
                yield break;

            float damper = 1.0f - (elapsed / duration);
            Vector3 randomOffset = Random.insideUnitSphere * magnitude * damper;

            camTransform.localPosition = new Vector3(originalPosition.x + randomOffset.x, originalPosition.y + randomOffset.y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (targetCam != null)
        {
            camTransform.localPosition = originalPosition;
        }

        shakeCoroutine = null;
        isShaking = false;
    }


    [Header("Camera Zoom")]
    public Transform player;
    public float movePerStep = 1f;
    public float stepDelay = 0.1f;
    public int numberOfSteps = 5;

    public static void MoveCameraTowardPlayer()
    {
        Camera mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogError("no main cam found");
            return;
        }

        CameraShake shaker = mainCam.GetComponent<CameraShake>();

        if (shaker == null)
        {
            shaker = mainCam.gameObject.AddComponent<CameraShake>();
        }

        shaker.StartCoroutine(shaker.StepMoveRoutine());
    }

    private IEnumerator StepMoveRoutine()
    {
        Transform camTransform = transform;

        for (int i = 0; i < numberOfSteps; i++)
        {
            Vector3 direction = player.position - camTransform.position;
            direction.z = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();
                camTransform.position += direction * movePerStep;
            }

            yield return new WaitForSeconds(stepDelay);
        }
    }
}
