using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterIntersectionController : MonoBehaviour
{
    [Header("Group A (North-South)")]
    public List<TrafficLight> carLightsA;
    public List<PedestrianTrafficTimer> pedLightsA;
    public List<GameObject> stopZonesA;

    [Header("Group B (East-West)")]
    public List<TrafficLight> carLightsB;
    public List<PedestrianTrafficTimer> pedLightsB;
    public List<GameObject> stopZonesB;

    [Header("Timing Settings")]
    public float groupAGreenTime = 10f;
    public float groupBGreenTime = 10f;
    public float yellowTransition = 2f;

    private void Start()
    {
        StartCoroutine(IntersectionLoop());
    }

    private IEnumerator IntersectionLoop()
    {
        while (true)
        {
            // Group A GREEN / Group B RED
            UpdateIntersection("Green", "Red");
            yield return new WaitForSeconds(groupAGreenTime);

            // Group A YELLOW
            UpdateIntersection("Yellow", "Red");
            yield return new WaitForSeconds(yellowTransition);

            // Group B GREEN / Group A RED
            UpdateIntersection("Red", "Green");
            yield return new WaitForSeconds(groupBGreenTime);

            // Group B YELLOW
            UpdateIntersection("Red", "Yellow");
            yield return new WaitForSeconds(yellowTransition);
        }
    }

    private void UpdateIntersection(string colorA, string colorB)
    {
        // Set Group A
        foreach (var c in carLightsA) if (c != null) c.SetStateManual(colorA);
        foreach (var p in pedLightsA) if (p != null) p.SetStateManual(colorA == "Red" ? "Green" : "Red");
        foreach (var z in stopZonesA) if (z != null) z.SetActive(colorA == "Red");

        // Set Group B
        foreach (var c in carLightsB) if (c != null) c.SetStateManual(colorB);
        foreach (var p in pedLightsB) if (p != null) p.SetStateManual(colorB == "Red" ? "Green" : "Red");
        foreach (var z in stopZonesB) if (z != null) z.SetActive(colorB == "Red");
    }
}