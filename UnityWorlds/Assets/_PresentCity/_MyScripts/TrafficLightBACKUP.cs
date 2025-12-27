using System.Collections;
using UnityEngine;

public class TrafficLightBACKUP : MonoBehaviour
{
    [Header("Material Settings")]
    [Tooltip("The original material used for the traffic light.")]
    public Material originalMat;

    [Tooltip("How bright the light should glow. Try values between 2 and 5.")]
    public float glowIntensity = 3.0f;

    [Header("State Textures")]
    [Tooltip("Texture for the green light.")]
    public Texture2D greenLight;
    [Tooltip("Texture for the yellow light.")]
    public Texture2D yellowLight;
    [Tooltip("Texture for the red light.")]
    public Texture2D redLight;

    private Material mat;

    public enum LightState { Off, On, Emergency }
    [Header("Current Configuration")]
    [Tooltip("The current state of the traffic light.")]
    public LightState currentState = LightState.Off;

    [Header("Timing Settings")]
    public float greenLightDuration = 5f;
    public float yellowLightDuration = 2f;
    public float redLightDuration = 5f;
    public float emergencyBlinkInterval = 0.5f;

    private Coroutine lightCoroutine;

    void Start()
    {
        // Create a unique material instance so we don't change every traffic light at once
        mat = Instantiate(originalMat);
        GetComponent<Renderer>().material = mat;

        SetLightState();
    }

    public void SetLightState()
    {
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
        }

        switch (currentState)
        {
            case LightState.Off:
                TurnOffLight();
                break;
            case LightState.On:
                lightCoroutine = StartCoroutine(LightCycle());
                break;
            case LightState.Emergency:
                lightCoroutine = StartCoroutine(EmergencyBlink());
                break;
        }
    }

    private IEnumerator LightCycle()
    {
        while (true)
        {
            // 1. RED STATE
            SetEmissionTexture(redLight);
            yield return new WaitForSeconds(redLightDuration);

            // --- RED TO GREEN: Skip Yellow ---

            // 2. GREEN STATE
            SetEmissionTexture(greenLight);
            yield return new WaitForSeconds(greenLightDuration);

            // 3. YELLOW STATE: Warning before Red
            SetEmissionTexture(yellowLight);
            yield return new WaitForSeconds(yellowLightDuration);
        }
    }

    private IEnumerator EmergencyBlink()
    {
        while (true)
        {
            SetEmissionTexture(yellowLight);
            yield return new WaitForSeconds(emergencyBlinkInterval);

            TurnOffEmission();
            yield return new WaitForSeconds(emergencyBlinkInterval);
        }
    }

    private void SetEmissionTexture(Texture2D lightTexture)
    {
        // Assign the PNG texture to the emission slot
        mat.SetTexture("_EmissionMap", lightTexture);

        // APPLY GLOW: We set the color to white but multiply it by our intensity
        // This forces the PNG texture into the HDR range for Bloom to catch it.
        mat.SetColor("_EmissionColor", Color.white * glowIntensity);

        mat.EnableKeyword("_EMISSION");
    }

    private void TurnOffEmission()
    {
        mat.SetColor("_EmissionColor", Color.black);
        mat.DisableKeyword("_EMISSION");
    }

    private void TurnOffLight()
    {
        TurnOffEmission();
    }
}