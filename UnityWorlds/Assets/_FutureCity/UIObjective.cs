using UnityEngine;
using TMPro;

public class UIObjective : MonoBehaviour
{
    public static UIObjective Instance { get; private set; }

    [SerializeField] TMP_Text objectiveText;
    [SerializeField] TMP_Text cardCounterText;

    void Awake()
    {
        Debug.Log("[UIObjective] Awake");
        if (Instance != null && Instance != this) {
            Debug.Log("[UIObjective] DESTROYED duplicate");
            Destroy(gameObject); return; }
        Instance = this;

        if (objectiveText) objectiveText.gameObject.SetActive(false);
        if (cardCounterText) cardCounterText.gameObject.SetActive(false);
    }

    public void SetObjective(string msg)
    {
        if (!objectiveText) return;
        objectiveText.text = msg;
        objectiveText.gameObject.SetActive(true);
    }

    public void SetCardCount(int have, int need)
    {
        if (!cardCounterText) return;
        cardCounterText.text = $"Cards: {have}/{need}";
        cardCounterText.gameObject.SetActive(true);
    }
}
