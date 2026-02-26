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
    public GameObject ironGate;

    [Header("Ρυθμίσεις Ανοίγματος Πύλης")]
    public float openHeight = 5f;
    public float openSpeed = 2f;

    [Header("Ήχοι")]
    public AudioSource gateSound; // Το Ηχείο μας!

    private bool isPlayerInRange = false;
    private bool isTalking = false;
    private bool isGateOpening = false;
    private Vector3 gateTargetPosition;

    void Start()
    {
        if (ironGate != null)
        {
            gateTargetPosition = ironGate.transform.position + new Vector3(0, openHeight, 0);
        }
    }

    void Update()
    {
        if (isGateOpening && ironGate != null)
        {
            ironGate.transform.position = Vector3.MoveTowards(ironGate.transform.position, gateTargetPosition, openSpeed * Time.deltaTime);
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                dialoguePanel.SetActive(true);

                if (PlayerInventory.hasSword == false)
                {
                    dialogueTextUI.text = dialogueNoSword;
                }
                else
                {
                    dialogueTextUI.text = dialogueWithSword;
                }

                Time.timeScale = 0f;
                isTalking = true;
            }
            else
            {
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f;
                isTalking = false;

                if (PlayerInventory.hasSword == true)
                {
                    if (!isGateOpening) // Για να είμαστε σίγουροι ότι θα παίξει μόνο ΜΙΑ φορά!
                    {
                        isGateOpening = true;

                        // ΠΑΙΞΕ ΤΟΝ ΗΧΟ!
                        if (gateSound != null)
                        {
                            gateSound.Play();
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) isPlayerInRange = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) { isPlayerInRange = false; dialoguePanel.SetActive(false); Time.timeScale = 1f; isTalking = false; } }
}