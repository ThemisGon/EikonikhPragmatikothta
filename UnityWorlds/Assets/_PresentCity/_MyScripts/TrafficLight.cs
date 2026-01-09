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

    void Start()
    {
        if (mat == null) InitializeMaterial();
    }

    private void InitializeMaterial()
    {
        if (originalMat != null)
        {
            mat = Instantiate(originalMat);
            GetComponent<Renderer>().material = mat;
        }
    }
    public void SetStateManual(string color)
    {
        if (mat == null) InitializeMaterial();

        switch (color.ToLower())
        {
            case "red": SetEmissionTexture(redLight); break;
            case "yellow": SetEmissionTexture(yellowLight); break;
            case "green": SetEmissionTexture(greenLight); break;
            default: TurnOffLight(); break;
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