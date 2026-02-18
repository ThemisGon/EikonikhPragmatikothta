using UnityEngine;

public class CardQuestManager : MonoBehaviour
{
    public static CardQuestManager Instance { get; private set; }

    [Header("Quest starts on game start")]
    public bool startQuestOnPlay = true;

    [Header("Quest")]
    public int requiredCards = 2;

    [Header("Progress (read-only)")]
    public int collectedCards;

    void Awake()
    {
        Debug.Log("[CardQuestManager] Awake");
        if (Instance != null && Instance != this) {
            Debug.Log("[CardQuestManager] DESTROYED duplicate");
            Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (startQuestOnPlay)
            StartQuest(requiredCards);
    }

    public void StartQuest(int required)
    {
        requiredCards = required;
        collectedCards = 0;

        // Show objective + counter permanently
        UIObjective.Instance?.SetObjective($"Collect {requiredCards} cards to escape");
        CardIconsUI.Instance?.UpdateIcons(collectedCards);

        // Popup message for 2-3 seconds
        PromptUI.Instance?.ShowTemp($"Find {requiredCards} cards to escape!", 3f);
    }

    public void AddCard()
    {
        collectedCards++;
        CardIconsUI.Instance?.UpdateIcons(collectedCards);
        if (collectedCards >= requiredCards)
        {
            UIObjective.Instance?.SetObjective("Head to the spaceship");
            PromptUI.Instance?.ShowTemp("All cards collected!", 2.5f);
        }
        else
        {
            PromptUI.Instance?.ShowTemp($"Card collected! ({collectedCards}/{requiredCards})", 2f);
        }
    }
}
