using System.Collections;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [Header("Material Settings")]
    public Material originalMat;
    public float glowIntensity = 30.0f;

    [Header("State Textures")]
    public Texture2D greenLight;
    public Texture2D yellowLight;
    public Texture2D redLight;

    private Material mat;

    public enum LightState { Off, On, Emergency, Manual }
    [Header("Current Configuration")]
    public LightState currentState = LightState.On;

    [Header("Timing Settings")]
    public float greenLightDuration = 5f;
    public float yellowLightDuration = 2f;
    public float redLightDuration = 5f;

    private Coroutine lightCoroutine;

    void Start()
    {
        // Initialize if not already done by SetStateManual
        if (mat == null) InitializeMaterial();
        SetLightState();
    }

    // New helper method to handle material setup safely
    private void InitializeMaterial()
    {
        if (originalMat != null)
        {
            mat = Instantiate(originalMat);
            GetComponent<Renderer>().material = mat;
        }
    }

    public void SetLightState()
    {
        if (lightCoroutine != null) StopCoroutine(lightCoroutine);

        if (currentState == LightState.On)
            lightCoroutine = StartCoroutine(LightCycle());
        else if (currentState == LightState.Off)
            TurnOffLight();
    }

    // UPDATED: This now checks for material initialization to prevent NullReference errors
    public void SetStateManual(string color)
    {
        if (mat == null) InitializeMaterial();

        currentState = LightState.Manual;
        if (lightCoroutine != null) StopCoroutine(lightCoroutine);

        switch (color.ToLower())
        {
            case "red": SetEmissionTexture(redLight); break;
            case "yellow": SetEmissionTexture(yellowLight); break;
            case "green": SetEmissionTexture(greenLight); break;
            default: TurnOffLight(); break;
        }
    }

    private IEnumerator LightCycle()
    {
        while (true)
        {
            SetEmissionTexture(redLight);
            yield return new WaitForSeconds(redLightDuration);
            SetEmissionTexture(greenLight);
            yield return new WaitForSeconds(greenLightDuration);
            SetEmissionTexture(yellowLight);
            yield return new WaitForSeconds(yellowLightDuration);
        }
    }

    private void SetEmissionTexture(Texture2D lightTexture)
    {
        if (mat != null && lightTexture != null)
        {
            mat.SetTexture("_EmissionMap", lightTexture);
            mat.SetColor("_EmissionColor", Color.white * glowIntensity);
            mat.EnableKeyword("_EMISSION");
        }
    }

    private void TurnOffLight()
    {
        if (mat != null)
        {
            mat.SetColor("_EmissionColor", Color.black);
            mat.DisableKeyword("_EMISSION");
        }
    }
}