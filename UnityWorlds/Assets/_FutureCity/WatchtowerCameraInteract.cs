using UnityEngine;

public class WatchtowerInteract : MonoBehaviour
{
    [Header("Cameras")]
    public Camera playerCamera;       // Main Camera
    public Camera watchtowerCamera;   // WatchtowerCamera (top view)

    [Header("Player control")]
    public MonoBehaviour playerMovement; // Your player movement script (disable when in watchtower)

    [Header("Messages")]
    public string interactMessage = "Press E to Interact";
    public string exitMessage = "Press E to Exit";

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange;
    private bool inWatchtowerView;

    void Start()
    {
        SetWatchtowerView(false);
    }

    void Update()
    {
        // Allow E only if player is in range or already in watchtower view
        if (!playerInRange && !inWatchtowerView) return;

        if (Input.GetKeyDown(interactKey))
        {
            SetWatchtowerView(!inWatchtowerView);
        }
    }

    void SetWatchtowerView(bool enable)
    {
        inWatchtowerView = enable;

        // Switch cameras
        if (playerCamera) playerCamera.enabled = !enable;
        if (watchtowerCamera) watchtowerCamera.enabled = enable;

        // Enable/disable player movement
        if (playerMovement) playerMovement.enabled = !enable;

        // Update global prompt
        if (PromptUI.Instance != null)
        {
            if (enable)
            {
                PromptUI.Instance.Show(exitMessage);
            }
            else
            {
                // If the player is still inside the trigger, show the interact message again
                if (playerInRange)
                    PromptUI.Instance.Show(interactMessage);
                else
                    PromptUI.Instance.Hide();
            }
        }

        // Cursor (optional)
        Cursor.visible = enable;
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        // If not in watchtower view, show interact prompt
        if (!inWatchtowerView && PromptUI.Instance != null)
            PromptUI.Instance.Show(interactMessage);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        // Hide prompt when leaving
        if (PromptUI.Instance != null)
            PromptUI.Instance.Hide();

        // If player leaves the trigger while in watchtower view, exit automatically
        if (inWatchtowerView)
            SetWatchtowerView(false);
    }
}
