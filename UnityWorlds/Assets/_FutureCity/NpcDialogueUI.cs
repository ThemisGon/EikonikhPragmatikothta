using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NpcDialogueUI : MonoBehaviour
{
    [Header("Dialogue Lines")]
    [TextArea(2, 4)]
    public string[] lines;

    [Header("Interact")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";
    public string promptMessage = "Press E to interact";

    [Header("Animator (optional)")]
    public Animator animator;
    public string isTalkingBoolName = "IsTalking";

    [Header("Input")]
    [SerializeField] private float interactCooldown = 0.2f; // prevents double-press / instant reopen

    private bool playerInRange;
    private bool dialogueStartedByThisNpc;
    private float nextInteractTime;

    void Reset()
    {
        // Auto-assign animator if on the same GameObject
        animator = GetComponent<Animator>();

        // Ensure collider is trigger
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Start()
    {
        // Make sure prompt is hidden initially
        HidePrompt();
    }

    void Update()
    {
        // Not in range -> nothing to do
        if (!playerInRange) return;

        // If any dialogue is open, hide prompt so it doesn't overlap
        if (DialogueUI.Instance != null && DialogueUI.Instance.IsOpen())
        {
            HidePrompt();
            return;
        }

        // Dialogue is NOT open and player is in range -> show prompt
        ShowPrompt();

        // Start dialogue on key press with cooldown
        if (Time.time >= nextInteractTime && Input.GetKeyDown(interactKey))
        {
            nextInteractTime = Time.time + interactCooldown;
            StartNpcDialogue();
        }
    }

    private void StartNpcDialogue()
    {
        if (lines == null || lines.Length == 0) return;

        HidePrompt();

        // Start talking animation while dialogue is open
        SetTalking(true);
        dialogueStartedByThisNpc = true;

        if (DialogueUI.Instance != null)
        {
            // Start dialogue and stop talking when finished
            DialogueUI.Instance.StartDialogue(lines, OnDialogueFinished);
        }
        else
        {
            Debug.LogWarning("[NpcDialogueUI] DialogueUI.Instance is null. Make sure DialogueUI exists in the scene.");
            SetTalking(false);
            dialogueStartedByThisNpc = false;
        }
    }

    private void OnDialogueFinished()
    {
        // Only react if THIS NPC started the dialogue
        if (!dialogueStartedByThisNpc) return;

        SetTalking(false);
        dialogueStartedByThisNpc = false;

        // Prevent immediate reopen on the same key press timing
        nextInteractTime = Time.time + interactCooldown;

        // If player is still close, show prompt again
        if (playerInRange)
            ShowPrompt();
        else
            HidePrompt();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = true;

        // Show prompt immediately if dialogue is not open
        if (DialogueUI.Instance == null || !DialogueUI.Instance.IsOpen())
            ShowPrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = false;
        HidePrompt();

        // If this NPC started the dialogue and player leaves, force close it
        if (dialogueStartedByThisNpc && DialogueUI.Instance != null && DialogueUI.Instance.IsOpen())
        {
            DialogueUI.Instance.ForceClose();
        }

        // Safety: stop talking state
        SetTalking(false);
        dialogueStartedByThisNpc = false;

        nextInteractTime = Time.time + interactCooldown;
    }

    private void SetTalking(bool talking)
    {
        if (animator == null || string.IsNullOrEmpty(isTalkingBoolName)) return;
        animator.SetBool(isTalkingBoolName, talking);
    }

    private void ShowPrompt()
    {
        // Use your global PromptUI singleton
        if (PromptUI.Instance != null)
            PromptUI.Instance.Show(promptMessage);
    }

    private void HidePrompt()
    {
        if (PromptUI.Instance != null)
            PromptUI.Instance.Hide();
    }
}
