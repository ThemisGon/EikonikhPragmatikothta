using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPCDialogueGiveCard : MonoBehaviour
{
    [Header("Prompt")]
    public string promptMessage = "Press E to interact";
    public KeyCode interactKey = KeyCode.E;

    [Header("Dialogue Lines")]
    [TextArea(2, 4)]
    public string[] dialogueLines;

    [Header("Card object to hide (in NPC hand)")]
    public GameObject cardObjectInHand;

    [Header("Pickup feedback")]
    public string pickedMessage = "Card collected!";
    public float pickedMessageSeconds = 2.0f;

    [Header("Sound")]
    public AudioClip pickupSfx;
    public float sfxVolume = 1f;

    [Header("Start Interaction SFX")]
    [SerializeField] private AudioClip startTalkSfx;
    [Range(0f, 3f)][SerializeField] private float startTalkVolume = 1f;

    bool playerInRange;
    bool cardGiven;
    bool dialogueStarted;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Update()
    {
        if (!playerInRange) return;
        if (cardGiven) return;

        // If dialogue UI is open, do nothing here (DialogueUI handles E)
        if (DialogueUI.Instance != null && DialogueUI.Instance.IsOpen()) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (dialogueStarted) return;
            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                // if not dialogue lines give the card imidiatelly
                OnDialogueFinishedGiveCard();
                return;
            }

            dialogueStarted = true;
PromptUI.Instance?.Hide();

// Play start interaction SFX via UI (2D)
if (DialogueUI.Instance != null && startTalkSfx != null)
    DialogueUI.Instance.PlayUISfx(startTalkSfx, startTalkVolume);

DialogueUI.Instance?.StartDialogue(dialogueLines, OnDialogueFinishedGiveCard);

        }
    }

    void OnDialogueFinishedGiveCard()
    {
        if (cardGiven) return;

        // Add card
        CardQuestManager.Instance?.AddCard();

        // Hide card mesh
        if (cardObjectInHand) cardObjectInHand.SetActive(false);

        // Sound
        if (pickupSfx && DialogueUI.Instance != null)
            DialogueUI.Instance.PlayUISfx(pickupSfx, sfxVolume);
        // Temp message
        PromptUI.Instance?.ShowTemp(pickedMessage, pickedMessageSeconds);

        cardGiven = true;

        // disable trigger + script so it never prompts again
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[NPC] Enter -> show prompt");
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (!cardGiven)
            PromptUI.Instance?.Show(promptMessage);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        PromptUI.Instance?.Hide();
    }
}
