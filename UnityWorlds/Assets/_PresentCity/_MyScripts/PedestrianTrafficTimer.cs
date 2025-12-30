using Seagull.City_03.SceneProps;
using System.Collections;
using UnityEngine;

public class PedestrianTrafficTimer : MonoBehaviour
{
    public GlowLight redLight;
    public GlowLight greenLight;

    [Header("Timing Settings")]
    public float greenLightDuration = 5f;
    public float redLightDuration = 5f;

    private Coroutine internalTimer;

    void Start()
    {
        internalTimer = StartCoroutine(TrafficLoop());
    }

    private IEnumerator TrafficLoop()
    {
        while (true)
        {
            greenLight.turnOn();
            redLight.turnOff();
            yield return new WaitForSeconds(greenLightDuration);

            greenLight.turnOff();
            redLight.turnOn();
            yield return new WaitForSeconds(redLightDuration);
        }
    }

    // NEW CLEANER FUNCTION FOR THE MASTER
    public void SetStateManual(string color)
    {
        // Stop the internal timer immediately
        if (internalTimer != null) StopCoroutine(internalTimer);
        this.enabled = false; // Disable this script component to prevent restarts

        if (color.ToLower() == "green")
        {
            greenLight.turnOn();
            redLight.turnOff();
        }
        else
        {
            greenLight.turnOff();
            redLight.turnOn();
        }
    }
}