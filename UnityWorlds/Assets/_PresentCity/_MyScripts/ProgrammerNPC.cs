using UnityEngine;
using UnityEngine.UI;

public class ProgrammerNPC : MonoBehaviour
{
    [Header("NPC Settings")]
    public int myID = 1;

    [Header("UI Connections")]
    public GameObject dialoguePanel;
    public Text mainTextField;
    public Text buttonAText;
    public Text buttonBText;
    public Text buttonCText;

    // --- NEW: The Interact Prompt ---
    public GameObject interactPrompt; // Drag your "Press E" text here
    // --------------------------------

    [Header("Riddle Data")]
    [TextArea] public string questionText = "Question here";
    public string optionA = "A";
    public string optionB = "B";
    public string optionC = "C";
    public int correctOption = 1;
    [TextArea] public string successHint = "Correct!";

    private bool isPlayerClose = false;

    void Update()
    {
        if (isPlayerClose && Input.GetKeyDown(KeyCode.E))
        {
            TryToTalk();
        }
    }

    void TryToTalk()
    {
        if (GameBrain.instance.currentStage == myID)
        {
            OpenRiddle();
        }
        else if (GameBrain.instance.currentStage < myID)
        {
            Debug.Log("I can't talk to you yet.");
        }
        else
        {
            Debug.Log("We are already done.");
        }
    }

    void OpenRiddle()
    {
        dialoguePanel.SetActive(true);
        if (interactPrompt != null) interactPrompt.SetActive(false); // Hide the "E" prompt while talking

        mainTextField.text = questionText;
        buttonAText.text = optionA;
        buttonBText.text = optionB;
        buttonCText.text = optionC;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClickedOption(int choice)
    {
        if (choice == correctOption)
        {
            mainTextField.text = successHint;
            GameBrain.instance.CompleteStage(); // THIS SAVES THE PROGRESS!

            // Hide buttons
            buttonAText.transform.parent.gameObject.SetActive(false);
            buttonBText.transform.parent.gameObject.SetActive(false);
            buttonCText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            mainTextField.text = "Error. Try again.";
        }
    }

    public void CloseWindow()
    {
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        // Show the "E" prompt again if we are still close
        if (isPlayerClose && interactPrompt != null) interactPrompt.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = true;
            if (interactPrompt != null) interactPrompt.SetActive(true); // Show "Press E"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = false;
            dialoguePanel.SetActive(false);
            if (interactPrompt != null) interactPrompt.SetActive(false); // Hide "Press E"
        }
    }
}