using UnityEngine;
using UnityEngine.InputSystem;

public class CarrotSack : MonoBehaviour
{
    public DialogueData d1;   
    public DialogueData d2;   

    public GameObject carrotSeeds;
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
        if (!playerInRange || !interact.WasPressedThisFrame()) return;

        if (!PlayerManager.hasCarrotSeeds)
        {
            DialogueManager.Instance.StartDialogue(d1, () =>
            {
                PlayerManager.hasCarrotSeeds = true;
                carrotSeeds.SetActive(true);
            });
        }
        else
        {
            DialogueManager.Instance.StartDialogue(d2);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInRange = false;
    }
}