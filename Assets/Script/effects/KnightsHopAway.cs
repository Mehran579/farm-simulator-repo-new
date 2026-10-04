using System.Collections;
using UnityEngine;

public class KnightsHopAway : MonoBehaviour
{
    public Transform destination;

    public float hopHeight = 0.5f;
    public float hopDuration = 0.3f;
    public int numberOfHops = 8;

    public void StartHopping()
    {
        StartCoroutine(Hop());
    }

    IEnumerator Hop()
    {
        Vector3 startPosition = transform.localPosition;

        Vector3 destinationPosition = transform.parent.InverseTransformPoint(destination.position);

        for (int i = 0; i < numberOfHops; i++)
        {
            Vector3 hopStart = Vector3.Lerp(
                startPosition,
                destinationPosition,
                (float)i / numberOfHops
            );

            Vector3 hopEnd = Vector3.Lerp(
                startPosition,
                destinationPosition,
                (float)(i + 1) / numberOfHops
            );

            float time = 0f;

            while (time < hopDuration)
            {
                time += Time.deltaTime;
                float t = time / hopDuration;

                Vector3 pos = Vector3.Lerp(hopStart, hopEnd, t);

                pos.y += Mathf.Sin(t * Mathf.PI) * hopHeight;

                transform.localPosition = pos;

                yield return null;
            }

            transform.localPosition = hopEnd;
        }
    }
}