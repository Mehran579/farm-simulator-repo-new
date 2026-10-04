using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public static event System.Action OnDialogueStarted;
    public static event System.Action OnDialogueEnded;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text speakerText;
    public TMP_Text bodyText;
    public Image portraitImage; 

    [Header("Settings")]
    public float typeSpeed = 0.03f;
    public InputAction advance; 
    Queue<DialogueLine> lines = new Queue<DialogueLine>();
    Coroutine typing;
    string currentFullText;
    bool isTyping;
    int startFrame = -1;


    public bool IsActive => dialoguePanel.activeSelf;

    void Reset()
    {
        advance = new InputAction("Advance", InputActionType.Button);
        advance.AddBinding("<Keyboard>/e");
        advance.AddBinding("<Mouse>/leftButton");
        advance.AddBinding("<Gamepad>/buttonSouth");
    }

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    void OnEnable() => advance.Enable();
    void OnDisable() => advance.Disable();

    void Update()
    {
        if (!IsActive || Time.frameCount == startFrame) return;

        if (advance.WasPressedThisFrame())
        {
            if (isTyping)
            {
                StopCoroutine(typing);
                bodyText.text = currentFullText;
                isTyping = false;
            }
            else ShowNextLine();
        }
    }

    System.Action onComplete;
    public void StartDialogue(DialogueData data, System.Action callback = null)
    {
        if (data == null || data.lines.Length == 0 || IsActive) return;
        this.onComplete = callback;
        startFrame = Time.frameCount; 
        lines.Clear();
        foreach (var line in data.lines) lines.Enqueue(line);
        dialoguePanel.SetActive(true);
        OnDialogueStarted?.Invoke();
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (lines.Count == 0) { EndDialogue(); return; }

        DialogueLine line = lines.Dequeue();
        speakerText.text = line.speaker;
        currentFullText = line.text;

        if (portraitImage != null)
        {
            portraitImage.sprite = line.portrait;
            portraitImage.enabled = line.portrait != null;
        }

        typing = StartCoroutine(TypeText(line.text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        bodyText.text = "";
        foreach (char c in text)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        OnDialogueEnded?.Invoke();
        onComplete?.Invoke();
    }
}