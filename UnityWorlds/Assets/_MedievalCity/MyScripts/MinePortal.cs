using UnityEngine;
using UnityEngine.SceneManagement; 
public class MinePortal : MonoBehaviour
{
    [Header("Το Όνομα της Επόμενης Σκηνής")]
    public string nextWorldName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerInventory.hasShield == true)
            {
                Debug.Log("Τηλεμεταφορά στον επόμενο κόσμο!");
                SceneManager.LoadScene(nextWorldName); // Φόρτωσε το άλλο παιχνίδι
            }
            else
            {
                Debug.Log("You can't win yet... Find the shield!");
            }
        }
    }
}