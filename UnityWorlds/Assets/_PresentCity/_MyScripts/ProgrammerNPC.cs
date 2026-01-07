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

    public GameObject interactPrompt;

    [Header("Riddle Data")]
    [TextArea] public string questionText = "Question here";
    public string optionA = "A";
    public string optionB = "B";
    public string optionC = "C";
    public int correctOption = 1;
    [TextArea] public string successHint = "Correct!";

    [TextArea] public string lockedMessage = "I can't talk to you yet. Help the others first.";
    [TextArea] public string alreadyDoneMessage = "We are already done here. Move along.";

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
        else
        {
            CheckStatusAndUpdateText();
        }
    }

    void OpenRiddle()
    {
        dialoguePanel.SetActive(true);
        if (interactPrompt != null) interactPrompt.SetActive(false); 

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
            GameBrain.instance.CompleteStage();

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


        if (isPlayerClose && interactPrompt != null)
        {
            interactPrompt.SetActive(true);
            CheckStatusAndUpdateText();
        }
    }

    void CheckStatusAndUpdateText()
    {
        if (interactPrompt == null) return;

        Text promptText = interactPrompt.GetComponent<Text>();
        if (promptText != null)
        {
            if (GameBrain.instance.currentStage < myID)
            {
                promptText.text = lockedMessage;
                promptText.color = Color.red;
            }
            else if (GameBrain.instance.currentStage > myID)
            {
                promptText.text = alreadyDoneMessage;
                promptText.color = Color.green;
            }
            else
            {
                promptText.text = "Press E to talk to the NPC";
                promptText.color = Color.white;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = true;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
                CheckStatusAndUpdateText();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = false;
            dialoguePanel.SetActive(false);
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}