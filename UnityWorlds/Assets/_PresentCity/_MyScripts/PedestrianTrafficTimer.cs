using UnityEngine;
using Seagull.City_03.SceneProps;
using System.Collections;

public class PedestrianTrafficTimer : MonoBehaviour
{
    [Header("Child References")]
    public GlowLight redLight;
    public GlowLight greenLight;

    [Header("Timing Settings")]
    public float greenDuration = 5f;
    public float redDuration = 5f;

    private void Start()
    {
        // Safety check to ensure you assigned the children in the Inspector
        if (redLight == null || greenLight == null)
        {
            Debug.LogError("PedestrianTrafficTimer: Please assign Red and Green GlowLight children!");
            return;
        }

        StartCoroutine(TrafficCycle());
    }

    private IEnumerator TrafficCycle()
    {
        while (true)
        {
            // STATE: Green (Walk)
            // Uses the turnOn/Off methods from your GlowLight script
            greenLight.turnOn();
            redLight.turnOff();
            yield return new WaitForSeconds(greenDuration);

            // STATE: Red (Stop)
            greenLight.turnOff();
            redLight.turnOn();
            yield return new WaitForSeconds(redDuration);
        }
    }
}