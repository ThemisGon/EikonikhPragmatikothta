using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [Header("Τα Λόγια του NPC")]
    [TextArea(3, 5)] public string dialogueNoWeapons;
    [TextArea(3, 5)] public string dialogueWithSword;
    [TextArea(3, 5)] public string dialogueWithShield;

    [Header("Σύνδεση με το UI & Ήχο")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextUI;

    public AudioSource voiceSound;

    private bool isPlayerInRange = false;
    private bool isTalking = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                dialoguePanel.SetActive(true);

                if (PlayerInventory.hasShield == true)
                {
                    dialogueTextUI.text = dialogueWithShield;
                }
                else if (PlayerInventory.hasSword == true)
                {
                    dialogueTextUI.text = dialogueWithSword;
                }
                else
                {
                    dialogueTextUI.text = dialogueNoWeapons;
                }

                if (voiceSound != null)
                {
                    voiceSound.Play();
                }

                Time.timeScale = 0f;
                isTalking = true;
            }
            else
            {
                dialoguePanel.SetActive(false);

                if (voiceSound != null)
                {
                    voiceSound.Stop();
                }

                Time.timeScale = 1f;
                isTalking = false;
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

            if (voiceSound != null) voiceSound.Stop();

            Time.timeScale = 1f;
            isTalking = false;
        }
    }
}