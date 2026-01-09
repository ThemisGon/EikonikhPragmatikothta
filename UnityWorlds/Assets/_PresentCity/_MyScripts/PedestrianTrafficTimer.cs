using UnityEngine;
using Seagull.City_03.SceneProps;

public class PedestrianTrafficTimer : MonoBehaviour
{
    public GlowLight redLight;
    public GlowLight greenLight;
    public void SetStateManual(string color)
    {

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