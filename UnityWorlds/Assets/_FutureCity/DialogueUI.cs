using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] GameObject root;          // Panel container (enable/disable)
    [SerializeField] TMP_Text dialogueText;    // Main dialogue text
    [SerializeField] TMP_Text continueText;    // "Press E to continue" (optional)

    [Header("Typing")]
    [SerializeField] float charsPerSecond = 40f;
    [SerializeField] KeyCode continueKey = KeyCode.E;

    [Header("Input")]
    [SerializeField] float inputCooldown = 0.12f; // Prevent double-press / instant reopen issues

    string[] lines;
    int index;
    bool isOpen;
    bool isTyping;
    bool lineFinished;
    Coroutine typingCo;

    Action onFinished;

    float nextInputTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Ensure UI starts hidden
        if (root) root.SetActive(false);
        if (continueText) continueText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isOpen) return;

        // Cooldown to avoid consuming the same E press multiple times
        if (Time.time < nextInputTime) return;

        if (Input.GetKeyDown(continueKey))
        {
            nextInputTime = Time.time + inputCooldown;

            if (isTyping)
            {
                // Skip typing -> show full line immediately
                FinishTypingInstant();
                return;
            }

            if (lineFinished)
            {
                NextLineOrFinish();
            }
        }
    }

    /// <summary>
    /// Starts a new dialogue. When it finishes, finishedCallback will be invoked.
    /// </summary>
    public void StartDialogue(string[] dialogueLines, Action finishedCallback = null)
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        // If another dialogue was already open, close it cleanly first
        InternalStopTyping();

        lines = dialogueLines;
        index = 0;
        onFinished = finishedCallback;

        isOpen = true;
        OpenUI();

        // Small cooldown so the E that started the dialogue doesn't instantly advance
        nextInputTime = Time.time + inputCooldown;

        ShowLine(lines[index]);
    }

    /// <summary>
    /// Force closes the dialogue (useful if player walks away).
    /// Still calls the finish callback so NPC can stop talking animations.
    /// </summary>
    public void ForceClose()
    {
        if (!isOpen) return;

        CloseUI();

        // Also block inputs for a moment to prevent immediate reopen
        nextInputTime = Time.time + inputCooldown;

        onFinished?.Invoke();
        onFinished = null;
    }

    void OpenUI()
    {
        if (root) root.SetActive(true);
        if (continueText) continueText.gameObject.SetActive(false);
        if (dialogueText) dialogueText.text = "";

        // Optional: unlock cursor while reading dialogue
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void CloseUI()
    {
        isOpen = false;

        InternalStopTyping();

        if (continueText) continueText.gameObject.SetActive(false);
        if (root) root.SetActive(false);

        // Optional: lock cursor back for gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ShowLine(string line)
    {
        InternalStopTyping();
        typingCo = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        lineFinished = false;
        if (continueText) continueText.gameObject.SetActive(false);

        if (dialogueText) dialogueText.text = "";

        float delay = 1f / Mathf.Max(1f, charsPerSecond);

        foreach (char c in line)
        {
            if (dialogueText) dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        lineFinished = true;
        if (continueText) continueText.gameObject.SetActive(true);
    }

    void FinishTypingInstant()
    {
        if (!isTyping) return;

        InternalStopTyping();

        if (dialogueText && lines != null && index >= 0 && index < lines.Length)
            dialogueText.text = lines[index];

        isTyping = false;
        lineFinished = true;

        if (continueText) continueText.gameObject.SetActive(true);
    }

    void NextLineOrFinish()
    {
        index++;

        // End reached -> close + callback
        if (lines == null || index >= lines.Length)
        {
            CloseUI();

            // Block immediate reopen
            nextInputTime = Time.time + inputCooldown;

            onFinished?.Invoke();
            onFinished = null;
            return;
        }

        ShowLine(lines[index]);
    }

    void InternalStopTyping()
    {
        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = null;

        isTyping = false;
        lineFinished = false;
    }

    public bool IsOpen() => isOpen;
}
