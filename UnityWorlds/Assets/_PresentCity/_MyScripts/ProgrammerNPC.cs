using UnityEngine;
using UnityEngine.UI;

public class ProgrammerNPC : MonoBehaviour
{
    [Header("NPC Settings")]
    public int myID = 1; // Set this to 1 for the first NPC, 2 for the second, etc.

    [Header("The Riddle")]
    [TextArea] public string questionText = "Write your question here.";
    public string optionA = "Option A";
    public string optionB = "Option B";
    public string optionC = "Option C";
    public int correctOption = 1; // 1 = A, 2 = B, 3 = C

    [Header("Messages")]
    [TextArea] public string successHint = "Correct! The next clue is near the fountain.";
    [TextArea] public string lockedMessage = "I can't talk to you yet. Help the others first.";
    [TextArea] public string alreadyDoneMessage = "System compiled successfully. You may proceed.";

    [Header("UI Connections (Drag & Drop)")]
    public GameObject dialoguePanel; // The big window background
    public Text mainTextField;       // The question text
    public Text buttonAText;         // Text on Button A
    public Text buttonBText;         // Text on Button B
    public Text buttonCText;         // Text on Button C

    private bool isPlayerClose = false;

    // This updates every frame to check for input
    void Update()
    {
        if (isPlayerClose && Input.GetKeyDown(KeyCode.E))
        {
            TryToTalk();
        }
    }

    void TryToTalk()
    {
        // 1. Check with the GameBrain: Is it my turn?
        int currentStage = GameBrain.instance.currentStage;

        if (currentStage == myID)
        {
            // IT IS MY TURN! Open the riddle.
            OpenRiddle();
        }
        else if (currentStage < myID)
        {
            // Too early (Trying to talk to NPC 3 when on Stage 1)
            Debug.Log(lockedMessage);
            // Ideally, you would show this text on screen, but for now we print to Console
        }
        else
        {
            // Too late (Already finished this NPC)
            Debug.Log(alreadyDoneMessage);
        }
    }

    void OpenRiddle()
    {
        dialoguePanel.SetActive(true); // Show the window
        mainTextField.text = questionText;
        buttonAText.text = optionA;
        buttonBText.text = optionB;
        buttonCText.text = optionC;

        // Unlock the mouse cursor so you can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Connect these functions to your Buttons in the Inspector!
    public void ClickedOption(int choice)
    {
        if (choice == correctOption)
        {
            // CORRECT!
            mainTextField.text = successHint; // Show the hint
            GameBrain.instance.CompleteStage(); // Tell the Brain we are done!

            // Hide the buttons so they can't click again
            buttonAText.transform.parent.gameObject.SetActive(false);
            buttonBText.transform.parent.gameObject.SetActive(false);
            buttonCText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            // WRONG!
            mainTextField.text = "Compilation Error (Wrong Answer). Try again.";
        }
    }

    public void CloseWindow()
    {
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor again for walking
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerClose = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = false;
            dialoguePanel.SetActive(false); // Close window if we walk away
        }
    }
}