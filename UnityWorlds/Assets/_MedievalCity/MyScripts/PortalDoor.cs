using UnityEngine;
using TMPro;

public class PortalDoor : MonoBehaviour
{
    [Header("Ρυθμίσεις Πόρτας")]
    public float openAngle = -90f; 
    public float smoothSpeed = 3f; 

    [Header("UI Μήνυμα (Αν είναι κλειδωμένη)")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextUI;
    [TextArea(2, 4)]
    public string lockedMessage = "A magic door keeps the door locked. You have to get the sword & the shield.";

    private bool isPlayerInRange = false;
    private bool isOpen = false;

    private Quaternion defaultRotation;
    private Quaternion openRotation;

    void Start()
    {
        defaultRotation = transform.rotation;
        openRotation = defaultRotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * smoothSpeed);
        }

        // Αν ο παίκτης είναι κοντά, πατήσει 'E' και η πόρτα είναι κλειστή...
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            if (PlayerInventory.hasShield == true)
            {
                isOpen = true;
                dialoguePanel.SetActive(false); 
            }
            else
            {
                dialoguePanel.SetActive(true);
                dialogueTextUI.text = lockedMessage;
            }
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) isPlayerInRange = true; }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialoguePanel.SetActive(false);
        }
    }
}