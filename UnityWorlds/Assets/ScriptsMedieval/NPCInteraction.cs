using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [Header("Τα Λόγια του NPC")]
    [TextArea(3, 5)]
    public string myDialogue;

    [Header("Σύνδεση με το UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTextUI;

    private bool isPlayerInRange = false;
    private bool isTalking = false; // ΝΕΟ: Θυμάται αν είμαστε ήδη σε συζήτηση!

    void Update()
    {
        // Αν ο παίκτης είναι κοντά ΚΑΙ πατήσει το πλήκτρο E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking) // Αν ΔΕΝ μιλάγαμε ήδη...
            {
                // ...Ξεκίνα τη συζήτηση!
                dialoguePanel.SetActive(true);
                dialogueTextUI.text = myDialogue;
                Time.timeScale = 0f; // ΠΑΓΩΝΕΙ ΤΟΝ ΧΡΟΝΟ!
                isTalking = true;
            }
            else // Αν μιλάγαμε ήδη και ξαναπάτησε το E...
            {
                // ...Κλείσε τον διάλογο!
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f; // ΞΕΠΑΓΩΝΕΙ ΤΟΝ ΧΡΟΝΟ!
                isTalking = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialoguePanel.SetActive(false);
            Time.timeScale = 1f; // Για σιγουριά, ο χρόνος συνεχίζει κανονικά αν φύγεις μακριά
            isTalking = false;
        }
    }
}