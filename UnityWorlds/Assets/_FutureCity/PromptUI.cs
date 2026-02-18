using UnityEngine;
using TMPro;
using System.Collections;

public class PromptUI : MonoBehaviour
{
    public static PromptUI Instance { get; private set; }

    [Header("Assign this in Inspector")]
    [SerializeField] private TMP_Text promptText;

    Coroutine tempRoutine;
    bool lockedByTemp;

    // Remember what the game *wants* to show when not in temp mode
    bool wantVisible;
    string wantMessage = "";

    void Awake()
    {
        Debug.Log("[PROMPTUI] Awake");
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        if (promptText == null) return;

        wantVisible = true;
        wantMessage = message ?? "";

        if (lockedByTemp) return;

        promptText.text = wantMessage;
        promptText.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (promptText == null) return;

        wantVisible = false;
        wantMessage = "";

        if (lockedByTemp) return;

        promptText.gameObject.SetActive(false);
    }

    public void ShowTemp(string message, float seconds)
    {
        if (promptText == null) return;

        if (tempRoutine != null) StopCoroutine(tempRoutine);
        tempRoutine = StartCoroutine(TempRoutine(message, seconds));
    }

    IEnumerator TempRoutine(string message, float seconds)
    {
        lockedByTemp = true;

        promptText.text = message ?? "";
        promptText.gameObject.SetActive(true);

        yield return new WaitForSeconds(seconds);

        lockedByTemp = false;
        tempRoutine = null;

        // Restore whatever the game wanted before the temp
        if (wantVisible && !string.IsNullOrEmpty(wantMessage))
        {
            promptText.text = wantMessage;
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }
}
