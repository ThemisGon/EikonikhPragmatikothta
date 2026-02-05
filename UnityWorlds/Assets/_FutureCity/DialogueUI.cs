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

    [Header("SFX")]
    [SerializeField] private AudioSource uiSfxSource;      // Optional: assign or auto-get
    [SerializeField] private AudioClip pressESfx;          // Plays on E press while dialogue is open
    [Range(0f, 3f)]
    [SerializeField] private float pressESfxVolume = 1f;
    [SerializeField] private bool sfxOnlyWhenAdvancingLine = true; // If true: only when going to next line (not on skip typing)

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
        if (uiSfxSource == null)
            uiSfxSource = GetComponent<AudioSource>(); // optional, if DialogueUI object has one

    }

    void Update()
    {
        if (Input.GetKeyDown(continueKey))
        {
            nextInputTime = Time.time + inputCooldown;

            // Case 1: currently typing -> E skips typing
            if (isTyping)
            {
                if (!sfxOnlyWhenAdvancingLine)
                    PlayPressESfx(); // optional: play also on skip

                FinishTypingInstant();
                return;
            }

            if (lineFinished)
            {
                // play SFX ONLY if there is another line
                if (index + 1 < lines.Length)
                    PlayPressESfx();

                NextLineOrFinish();
            }
        }
    }
    public void PlayUISfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        if (uiSfxSource == null)
            uiSfxSource = GetComponent<AudioSource>();

        if (uiSfxSource == null)
            uiSfxSource = gameObject.AddComponent<AudioSource>();

        uiSfxSource.playOnAwake = false;
        uiSfxSource.loop = false;
        uiSfxSource.spatialBlend = 0f; // 2D

        uiSfxSource.PlayOneShot(clip, volume);
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
    private void PlayPressESfx()
    {
        if (pressESfx == null) return;

        // If no source assigned, use PlayClipAtPoint as a fallback (2D-ish if listener is near camera)
        if (uiSfxSource == null)
        {
            AudioSource.PlayClipAtPoint(pressESfx, Vector3.zero, pressESfxVolume);
            return;
        }

        uiSfxSource.PlayOneShot(pressESfx, pressESfxVolume);
    }


}

