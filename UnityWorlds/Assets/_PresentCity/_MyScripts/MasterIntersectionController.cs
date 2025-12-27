using UnityEngine;
using System.Collections;
using Seagull.City_03.SceneProps;

public class MasterIntersectionController : MonoBehaviour
{
    [Header("Lights")]
    public TrafficLight carLight;
    public PedestrianTrafficTimer pedLight; // Uses your standalone script

    [Header("Logic Settings")]
    public bool carIsPriority = true;
    public float carGreenTime = 10f;
    public float pedGreenTime = 7f;
    public float yellowTransition = 2f;

    [Header("AI Control")]
    public GameObject stopZone; // A collider that cars will detect

    private void Start()
    {
        // Stop the individual timers on the lights so they don't fight this controller
        carLight.currentState = TrafficLight.LightState.Off;
        pedLight.enabled = false;

        StartCoroutine(IntersectionLoop());
    }

    private IEnumerator IntersectionLoop()
    {
        while (true)
        {
            // STATE 1: CARS MOVING (Car Green, Ped Red)
            stopZone.SetActive(false); // Let cars pass
            carLight.SetStateManual("Green"); // We will add this small method to your script
            pedLight.redLight.turnOn();
            pedLight.greenLight.turnOff();
            yield return new WaitForSeconds(carGreenTime);

            // STATE 2: CARS WARNING (Car Yellow, Ped Red)
            carLight.SetStateManual("Yellow");
            yield return new WaitForSeconds(yellowTransition);

            // STATE 3: PEDESTRIANS CROSSING (Car Red, Ped Green)
            stopZone.SetActive(true); // Tell cars to stop
            carLight.SetStateManual("Red");
            pedLight.redLight.turnOff();
            pedLight.greenLight.turnOn();
            yield return new WaitForSeconds(pedGreenTime);
        }
    }
}