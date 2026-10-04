using UnityEngine;

public class ConstRotation : MonoBehaviour
{
    public bool useUnscaledTime = false;

    public float minSpeed = 5f;
    public float maxSpeed = 15f;
    public float wobbleFrequency = 1f; // Speed of variation

    void Update()
    {
        float time = useUnscaledTime ? Time.unscaledTime : Time.time;
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        // PerlinNoise generates smooth continuous randomness between 0 and 1
        float noise = Mathf.PerlinNoise(time * wobbleFrequency, 0f);
        float currentZSpeed = Mathf.Lerp(minSpeed, maxSpeed, noise);

        transform.Rotate(new Vector3(0f, 0f, currentZSpeed) * deltaTime, Space.Self);
    }
}