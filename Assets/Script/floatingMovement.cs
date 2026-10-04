using UnityEngine;
public class floatingMovement : MonoBehaviour
{
    public float height = 0.15f;
    public float frequency = 2.0f;

    private float initialYPosition;

    void Start()
    {
        initialYPosition = transform.localPosition.y;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * height;
        Vector3 newPosition = new Vector3(transform.localPosition.x, initialYPosition + yOffset, transform.localPosition.z);
        transform.localPosition = newPosition;
    }
}
