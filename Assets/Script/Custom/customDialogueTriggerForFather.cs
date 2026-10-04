using UnityEngine;
using UnityEngine.InputSystem;

public class customDialogueTriggerForFather : MonoBehaviour
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
        {
            exclampatory.SetActive(false);
            TriggerDialogue();
        }
    }
    public GameObject exclampatory;
    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, () => 
        {
            CameraShake.TriggerShake(0.5f, 0.5f);
            DialogueManager.Instance.StartDialogue(_3rdFatherDialogue, () =>
            {
                fenceDoor.GetComponent<SpriteRenderer>().sprite = openDoorSprite;
                fenceDoor.GetComponent<Collider2D>().enabled = false;
                fatherDialogue.dialogue = lastDialogue;
                fatherDialogue.enabled = true;
                this.enabled = false;
                //PlayerManager.Instance.currentCutSceneState = PlayerManager.CutSceneState.GoToFather;
            });
        });
    }
    public DialogueData lastDialogue;
    public DialogueTrigger fatherDialogue;
    public GameObject fenceDoor;
    public Sprite openDoorSprite;
    public DialogueData _3rdFatherDialogue;
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