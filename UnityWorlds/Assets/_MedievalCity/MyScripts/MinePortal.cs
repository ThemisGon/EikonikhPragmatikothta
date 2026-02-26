using UnityEngine;
using UnityEngine.SceneManagement; // Απαραίτητο για να αλλάζουμε πίστες/κόσμους!

public class MinePortal : MonoBehaviour
{
    [Header("Το Όνομα της Επόμενης Σκηνής")]
    public string nextWorldName;

    private void OnTriggerEnter(Collider other)
    {
        // Αν μας ακουμπήσει ο Παίκτης...
        if (other.CompareTag("Player"))
        {
            // ...και ΕΧΕΙ την ασπίδα (άρα έχει ολοκληρώσει το χωριό)
            if (PlayerInventory.hasShield == true)
            {
                Debug.Log("Τηλεμεταφορά στον επόμενο κόσμο!");
                SceneManager.LoadScene(nextWorldName); // Φόρτωσε το άλλο μενού/παιχνίδι!
            }
            else
            {
                Debug.Log("You can't win yet... Find the shield!");
            }
        }
    }
}