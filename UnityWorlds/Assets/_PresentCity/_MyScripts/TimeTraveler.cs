using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTraveler : MonoBehaviour
{
    public string sceneToLoad = "FutureScene"; // The exact name of your future scene

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Teleporting to the Future...");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}