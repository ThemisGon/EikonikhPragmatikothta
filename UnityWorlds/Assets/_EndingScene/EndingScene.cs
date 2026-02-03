using UnityEngine;

public class EndingSceneExit : MonoBehaviour
{
    public KeyCode exitKey = KeyCode.E;

    void Update()
    {
        if (Input.GetKeyDown(exitKey))
        {
            QuitGame();
        }
    }

    void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
