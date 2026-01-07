using System.Collections;
using UnityEngine;

public class TrafficLightBACKUP : MonoBehaviour
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
        mat = Instantiate(originalMat);
        GetComponent<Renderer>().material = mat;
        SetLightState();
    }

    public void SetLightState()
    {
        if (lightCoroutine != null) StopCoroutine(lightCoroutine);

        if (currentState == LightState.On)
            lightCoroutine = StartCoroutine(LightCycle());
        else if (currentState == LightState.Off)
            TurnOffLight();
    }

    public void SetStateManual(string color)
    {
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
        mat.SetTexture("_EmissionMap", lightTexture);
        mat.SetColor("_EmissionColor", Color.white * glowIntensity);
        mat.EnableKeyword("_EMISSION");
    }

    private void TurnOffLight()
    {
        mat.SetColor("_EmissionColor", Color.black);
        mat.DisableKeyword("_EMISSION");
    }
}