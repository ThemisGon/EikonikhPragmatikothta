using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SpaceshipExitInteract : MonoBehaviour
{
    public string promptMessage = "Press E to escape";
    public KeyCode key = KeyCode.E;

    [Header("If not enough cards")]
    public string needCardsMessage = "You need 2 cards to leave!";
    public float needMsgSeconds = 2.5f;

    [Header("Win")]
    public string winSceneName = "GameOver"; // Scene name

    bool playerInRange;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(key))
        {
            int have = CardQuestManager.Instance ? CardQuestManager.Instance.collectedCards : 0;
            int need = CardQuestManager.Instance ? CardQuestManager.Instance.requiredCards : 2;

            if (have >= need)
            {
                SceneManager.LoadScene(winSceneName);
            }
            else
            {
                PromptUI.Instance?.ShowTemp(needCardsMessage, needMsgSeconds);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        PromptUI.Instance?.Show(promptMessage);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        PromptUI.Instance?.Hide();
    }
}
