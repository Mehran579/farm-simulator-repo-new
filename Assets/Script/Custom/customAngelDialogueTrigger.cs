using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class customAngelDialogueTrigger : MonoBehaviour
{
    public DialogueData dialogue;
    public bool requireInput = true;
    public InputAction interact;

    bool playerInRange;

    void Reset()
    {
        interact = new InputAction("Interact", InputActionType.Button);
        interact.AddBinding("<Keyboard>/e");
        interact.AddBinding("<Gamepad>/buttonSouth");
    }

    void OnEnable() => interact.Enable();
    void OnDisable() => interact.Disable();

    void Update()
    {
        if (requireInput && playerInRange && interact.WasPressedThisFrame())
            TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, () =>
        {
            StartCoroutine(flash());
            PlayerManager.canMove = false;
        });
    }


    public ScreenFade flashscreen;
    public DialogueData angeld2;
    public DialogueData angeld3;
    IEnumerator flash()
    {
        yield return StartCoroutine(flashscreen.FadeToBlack());
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(flashscreen.FadeFromBlack());
        DialogueManager.Instance.StartDialogue(angeld2, () =>
        {
            StartCoroutine(flash2());
        });
    }
    IEnumerator flash2()
    {
        yield return StartCoroutine(flashscreen.FadeToBlack());
        
        foreach (var item in angelParts)
        {
            item.enabled = false;
        }
        angelhoppinaway.StartHopping();

        for (int i = 0; i < Npc.Length; i++)
        {
            Npc[i].position = NpcToMoveToPoints[i].position;
            Npc[i].GetComponent<RandomNudge>().ResetNudgePosition();
        }

        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(flashscreen.FadeFromBlack());
        
        DialogueManager.Instance.StartDialogue(angeld3, () =>
        {
            PlayerManager.hasTalkedToBiblicallyAccurateAngel = true;
            PlayerManager.canMove = true;
            GetComponent<Collider2D>().enabled = false;
            Destroy(angel,1f);
        });
    }

    public GameObject angel;

    public Transform[] Npc;
    public Transform[] NpcToMoveToPoints;
    public HopAway angelhoppinaway;
    public Collider2D[] angelParts;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (!requireInput) TriggerDialogue();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

}