using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTraveler : MonoBehaviour
{
    public string sceneToLoad = "FutureScene";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Teleporting to the Future...");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}