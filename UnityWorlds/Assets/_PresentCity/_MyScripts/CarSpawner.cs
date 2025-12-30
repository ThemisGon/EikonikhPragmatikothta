using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform[] routeWaypoints;
    public float spawnInterval = 5f; // I set this to 5 by default for you

    [Header("Anti-Collision Settings")]
    public float clearanceRadius = 4f;
    public LayerMask obstacleLayer;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 1. Look for ALL colliders in the spawn area
            Collider[] hits = Physics.OverlapSphere(routeWaypoints[0].position, clearanceRadius, obstacleLayer);

            bool isBlockedByCar = false;
            foreach (var hit in hits)
            {
                // 2. The Filter: Only count it if it has a CarAI script (meaning it's a car)
                // We also ignore the car we *just* spawned if it hasn't moved yet? No, that's fine.
                if (hit.GetComponent<CarAI>() != null || hit.GetComponentInParent<CarAI>() != null)
                {
                    isBlockedByCar = true;
                    break;
                }
            }

            if (!isBlockedByCar)
            {
                // 3. Clear! Spawn the car.
                GameObject newCar = Instantiate(carPrefab, routeWaypoints[0].position, routeWaypoints[0].rotation);

                CarAI ai = newCar.GetComponent<CarAI>();
                if (ai != null)
                {
                    ai.waypoints = routeWaypoints;
                    ai.destroyAtEnd = true;
                }
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                // Blocked by another car! Wait 1 second and check again.
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (routeWaypoints != null && routeWaypoints.Length > 0 && routeWaypoints[0] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(routeWaypoints[0].position, clearanceRadius);
        }
    }
}