using UnityEngine;
using TMPro;

public class ShieldNPC : MonoBehaviour
{
    [Header("Διάλογοι")]
    [TextArea(3, 5)] public string dialogueBefore; // Τι λέει ΠΡΙΝ σου δώσει την ασπίδα
    [TextArea(3, 5)] public string dialogueAfter;  // Τι λέει ΑΦΟΥ σου τη δώσει

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextUI;
    public GameObject visualShield;
    private bool isPlayerInRange = false;
    private bool isTalking = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                dialoguePanel.SetActive(true);

                // ΕΛΕΓΧΟΣ: Έχει ήδη την ασπίδα ο παίκτης;
                if (PlayerInventory.hasShield == false)
                {
                    dialogueTextUI.text = dialogueBefore; 
                    PlayerInventory.hasShield = true;
                    if (visualShield != null) visualShield.SetActive(true);
                }
                else
                {
                    dialogueTextUI.text = dialogueAfter; // Έχει ήδη την ασπίδα
                }

                Time.timeScale = 0f;
                isTalking = true;
            }
            else
            {
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f;
                isTalking = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) isPlayerInRange = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) { isPlayerInRange = false; dialoguePanel.SetActive(false); Time.timeScale = 1f; isTalking = false; } }
}