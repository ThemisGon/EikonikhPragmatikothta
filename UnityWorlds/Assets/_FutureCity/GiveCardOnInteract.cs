using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GiveCardOnInteract : MonoBehaviour
{
    [Header("Prompt")]
    public string promptMessage = "Press E to interact";
    public KeyCode key = KeyCode.E;

    [Header("Card object to hide (in NPC hand)")]
    public GameObject cardObjectInHand;   // drag the CardMedival here

    [Header("Pickup feedback")]
    public string pickedMessage = "Card collected!";
    public float pickedMessageSeconds = 2.5f;

    [Header("Sound")]
    public AudioClip pickupSfx;
    public float sfxVolume = 1f;

    bool playerInRange;
    bool cardGiven;

    void Reset()
    {
        // make sure trigger is enabled
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Update()
    {
        if (!playerInRange || cardGiven) return;

        if (Input.GetKeyDown(key))
        {
            // Give the card (count it)
            CardQuestManager.Instance?.AddCard();

            // Hide card mesh in hand
            if (cardObjectInHand) cardObjectInHand.SetActive(false);

            // Play sound (no need for AudioSource on NPC)
            if (pickupSfx) AudioSource.PlayClipAtPoint(pickupSfx, transform.position, sfxVolume);

            // Show temp message
            PromptUI.Instance?.ShowTemp(pickedMessage, pickedMessageSeconds);

            cardGiven = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[GIVE] Enter -> show prompt");

        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (!cardGiven)
            PromptUI.Instance?.Show(promptMessage);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        // hide only if we are not showing a temp message
        PromptUI.Instance?.Hide();
    }
}
