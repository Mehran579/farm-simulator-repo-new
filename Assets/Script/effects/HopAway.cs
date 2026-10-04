using System.Collections;
using UnityEngine;

public class HopAway : MonoBehaviour
{
    public float hopDistance = 1f;
    public float hopHeight = 0.5f;
    public float hopDuration = 0.3f;
    public int numberOfHops = 8;

    public void StartHopping()
    {
        StartCoroutine(Hop());
    }

    IEnumerator Hop()
    {
        for (int i = 0; i < numberOfHops; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.right * hopDistance;
            float time = 0;
            while (time < hopDuration)
            {
                time += Time.deltaTime;
                float t = time / hopDuration;
                Vector3 pos = Vector3.Lerp(startPos, endPos, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * hopHeight;
                transform.position = pos;
                yield return null;
            }
            transform.position = endPos;
        }
    }
}