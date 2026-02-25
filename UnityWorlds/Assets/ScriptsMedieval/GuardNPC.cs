using UnityEngine;
using TMPro;

public class GuardNPC : MonoBehaviour
{
    [Header("Διάλογοι")]
    [TextArea(3, 5)] public string dialogueNoSword;
    [TextArea(3, 5)] public string dialogueWithSword;

    [Header("UI και Πύλη")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextUI;
    public GameObject ironGate; // Η μεταλλική πύλη

    [Header("Ρυθμίσεις Ανοίγματος Πύλης")]
    public float openHeight = 5f; // Πόσα μέτρα προς τα πάνω θα ανέβει
    public float openSpeed = 2f;  // Πόσο γρήγορα θα ανεβαίνει

    private bool isPlayerInRange = false;
    private bool isTalking = false;

    // Μυστικές μεταβλητές για την κίνηση της πύλης
    private bool isGateOpening = false;
    private Vector3 gateTargetPosition;

    void Start()
    {
        // Με το που ξεκινάει το παιχνίδι, υπολογίζει πού πρέπει να φτάσει η πύλη (π.χ. 5 μέτρα πιο πάνω)
        if (ironGate != null)
        {
            gateTargetPosition = ironGate.transform.position + new Vector3(0, openHeight, 0);
        }
    }

    void Update()
    {
        // ΑΝ έχει δοθεί η εντολή να ανοίξει, σήκωνε την πύλη σιγά-σιγά κάθε καρέ του παιχνιδιού!
        if (isGateOpening && ironGate != null)
        {
            ironGate.transform.position = Vector3.MoveTowards(ironGate.transform.position, gateTargetPosition, openSpeed * Time.deltaTime);
        }

        // Σύστημα Διαλόγου
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking) // Αν ανοίγουμε τον διάλογο
            {
                dialoguePanel.SetActive(true);

                if (PlayerInventory.hasSword == false)
                {
                    dialogueTextUI.text = dialogueNoSword;
                }
                else
                {
                    dialogueTextUI.text = dialogueWithSword;
                    // Προσοχή: Δεν την εξαφανίζουμε πια εδώ! Περιμένουμε να κλείσει ο διάλογος.
                }

                Time.timeScale = 0f; // Παγώνει ο χρόνος
                isTalking = true;
            }
            else // Αν ξαναπατήσουμε το E για να κλείσουμε τον διάλογο
            {
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f; // Ξεπαγώνει ο χρόνος
                isTalking = false;

                // ΑΝ έχουμε το σπαθί, δώσε το σύνθημα να ξεκινήσει η πύλη να ανεβαίνει!
                if (PlayerInventory.hasSword == true)
                {
                    isGateOpening = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) isPlayerInRange = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) { isPlayerInRange = false; dialoguePanel.SetActive(false); Time.timeScale = 1f; isTalking = false; } }
}