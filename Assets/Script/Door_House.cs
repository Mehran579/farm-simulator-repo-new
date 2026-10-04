using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door_House : MonoBehaviour
{
    public Transform toSpawnPos;
    public Camera newCam;
    public GameObject nextLevel;
    public GameObject currentLevel;
    public ScreenFade screenFade;

    private bool transitioning;

    void Start()
    {
        nextLevel.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Keyboard.current.eKey.isPressed &&
            collision.CompareTag("Player") &&
            !transitioning)
        {
            StartCoroutine(ChangeLevel(collision));
        }
    }

    public IEnumerator ChangeLevel(Collider2D collision)
    {
        transitioning = true;
        PlayerManager.canMove = false;

        yield return StartCoroutine(screenFade.FadeToBlack());

        nextLevel.SetActive(true);
        currentLevel.SetActive(false);

        Camera oldcam = Camera.main;

        newCam.gameObject.tag = "MainCamera";
        newCam.gameObject.SetActive(true);

        oldcam.tag = "Untagged";
        oldcam.gameObject.SetActive(false);

        collision.transform.position = toSpawnPos.position;

        yield return StartCoroutine(screenFade.FadeFromBlack());

        PlayerManager.canMove = true;
        transitioning = false;
    }
}