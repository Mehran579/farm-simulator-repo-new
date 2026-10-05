using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    Vector2 MoveInput;
    public float MoveSpeed;
    public static bool canMove = true;
    Rigidbody2D rb;
    public enum PlayerState { Move, Cutscene, MovingCutScene, knockback };
    public PlayerState currentState;


    public enum CutSceneState { GoToFather };
    public CutSceneState currentCutSceneState;

    public static bool hasCarrotSeeds;
    public GameObject carrotseed;

    public static bool hasTalkedToBiblicallyAccurateAngel;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cropTrigger.carrotSeed = carrotseed;
    }
    void OnEnable()
    {
        DialogueManager.OnDialogueStarted += EnterCutscene;
        DialogueManager.OnDialogueEnded += ExitCutscene;
    }

    void OnDisable()
    {
        DialogueManager.OnDialogueStarted -= EnterCutscene;
        DialogueManager.OnDialogueEnded -= ExitCutscene;
    }
    void EnterCutscene()
    {
        currentState = PlayerState.Cutscene;
        rb.linearVelocity = Vector2.zero;
    }

    void ExitCutscene() => currentState = PlayerState.Move;
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerState.Move:
                if (canMove)
                {
                    rb.linearVelocity = MoveInput * MoveSpeed;
                }
                break;

            case PlayerState.Cutscene:
                rb.linearVelocity = Vector2.zero;
                break;

            case PlayerState.MovingCutScene:
                PlayCutScene(currentCutSceneState);
                break;
            case PlayerState.knockback:
                break;
        }
    }
    bool inMovingCUtscene;


    void PlayCutScene(CutSceneState cutscene)
    {
        switch (cutscene)
        {
            case CutSceneState.GoToFather:
                if (!inMovingCUtscene)
                {
                    rb.linearVelocity = Vector2.zero;
                    inMovingCUtscene = true;
                    StartCoroutine(goToFather());
                }
                break;
        }
    }

    public Transform fatherPosition;
    public GameObject exclamatipon;
    public DialogueData fatherDialogue1;
    IEnumerator goToFather()
    {
        yield return new WaitForSeconds(1f);
        while (Vector2.Distance(transform.position, fatherPosition.position) > 0.1f)
        {
            rb.MovePosition(Vector2.MoveTowards(transform.position, fatherPosition.position, MoveSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        rb.linearVelocity = Vector2.zero;

        rb.position = fatherPosition.position;
        exclamatipon.SetActive(false);
        DialogueManager.Instance.StartDialogue(fatherDialogue1);
    }
    //private void Start()
    //{
    //    TEMPmarketDoor.StartCoroutine(TEMPmarketDoor.ChangeLevel(GetComponent<Collider2D>()));
    //}
    //public Door_House TEMPmarketDoor;
}
