using UnityEngine;
using TMPro;

public class SwordNPC : MonoBehaviour
{
    [Header("Διάλογοι")]
    [TextArea(3, 5)] public string dialogueBefore;
    [TextArea(3, 5)] public string dialogueAfter;  

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextUI;
    public GameObject visualSword;
    private bool isPlayerInRange = false;
    private bool isTalking = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                dialoguePanel.SetActive(true);

                if (PlayerInventory.hasSword == false)
                {
                    dialogueTextUI.text = dialogueBefore;
                    PlayerInventory.hasSword = true;
                    if (visualSword != null) visualSword.SetActive(true);
                }
                else
                {
                    dialogueTextUI.text = dialogueAfter; 
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