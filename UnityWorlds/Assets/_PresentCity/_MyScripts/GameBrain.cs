using UnityEngine;

public class GameBrain : MonoBehaviour
{
    public static GameBrain instance;

    [Header("Game State")]
    public int currentStage = 1;

    [Header("The Reward")]
    public GameObject portalObject;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (portalObject != null)
        {
            portalObject.SetActive(false);
        }
    }

    public void CompleteStage()
    {
        currentStage++;

        Debug.Log("Quest Advanced! Now at Stage: " + currentStage);

        if (currentStage > 3)
        {
            OpenPortal();
        }
    }

    void OpenPortal()
    {
        Debug.Log("Portal Opened!");
        if (portalObject != null)
        {
            portalObject.SetActive(true);
        }
    }
}