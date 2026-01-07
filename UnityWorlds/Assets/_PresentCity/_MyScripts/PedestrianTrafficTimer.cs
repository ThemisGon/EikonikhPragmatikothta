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

    public void SetStateManual(string color)
    {
        if (internalTimer != null) StopCoroutine(internalTimer);
        this.enabled = false;

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