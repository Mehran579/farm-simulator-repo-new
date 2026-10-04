using UnityEngine;

public class floatingSize : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 initailsize;
    public float frequency;
    public float height;
    void Start()
    {
        initailsize = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float hoffset = Mathf.Sin(Time.time * frequency) * height;
        transform.localScale = initailsize * (1f + hoffset);
    }
}
