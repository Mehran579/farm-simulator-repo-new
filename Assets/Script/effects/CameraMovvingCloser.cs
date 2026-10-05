using UnityEngine;
using System.Collections;

public class CameraMovvingCloser : MonoBehaviour
{
    public float movePerStep = 1f;
    public float zoomPerStep = 0.5f;
    public float stepDelay = 0.1f;
    public int numberOfSteps = 5;

    private Transform player;

    private Vector3 zoomOriginalPosition;
    private float zoomOriginalSize;

    private void OnEnable()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;


        //CameraMovvingCloser.MoveCameraTowardPlayer();
    }

    public static void MoveCameraTowardPlayer()
    {
        Camera mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogError("No Main Camera found.");
            return;
        }

        CameraMovvingCloser mover = mainCam.GetComponent<CameraMovvingCloser>();

        if (mover == null)
        {
            mover = mainCam.gameObject.AddComponent<CameraMovvingCloser>();
        }

        if (mover.player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");

            if (playerObject == null)
            {
                Debug.LogError("No Player found.");
                return;
            }

            mover.player = playerObject.transform;
        }

        mover.StartCoroutine(mover.StepMoveRoutine());
    }

    private IEnumerator StepMoveRoutine()
    {
        Transform cam = transform;
        zoomOriginalPosition = cam.position;
        zoomOriginalSize = GetComponent<Camera>().orthographicSize;

        Camera cameraComponent = GetComponent<Camera>();

        for (int i = 0; i < numberOfSteps; i++)
        {
            Vector3 direction = player.position - cam.position;
            direction.z = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();
                cam.position += direction * movePerStep;
            }

            cam.position = new Vector3(
                cam.position.x,
                cam.position.y,
                -10f
            );

            cameraComponent.orthographicSize -= zoomPerStep;

            cameraComponent.orthographicSize =
                Mathf.Max(cameraComponent.orthographicSize, 0.1f);

            yield return new WaitForSeconds(stepDelay);
        }
    }

    public static void MoveCameraBack()
    {
        Camera mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogError("No Main Camera found.");
            return;
        }

        CameraMovvingCloser mover =
            mainCam.GetComponent<CameraMovvingCloser>();

        if (mover == null)
        {
            Debug.LogError("CameraMovvingCloser is not on the Main Camera.");
            return;
        }

        mover.StartCoroutine(mover.StepMoveBackRoutine());
    }

    private IEnumerator StepMoveBackRoutine()
    {
        Transform cam = transform;
        Camera cameraComponent = GetComponent<Camera>();

        for (int i = 0; i < numberOfSteps; i++)
        {
            Vector3 direction = zoomOriginalPosition - cam.position;
            direction.z = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();
                cam.position += direction * movePerStep;
            }

            cam.position = new Vector3(
                cam.position.x,
                cam.position.y,
                -10f
            );

            cameraComponent.orthographicSize += zoomPerStep;

            yield return new WaitForSeconds(stepDelay);
        }

        cam.position = new Vector3(
            zoomOriginalPosition.x,
            zoomOriginalPosition.y,
            -10f
        );

        cameraComponent.orthographicSize = zoomOriginalSize;
    }
}