using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform[] routeWaypoints;
    public float spawnInterval = 5f; 

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
            Collider[] hits = Physics.OverlapSphere(routeWaypoints[0].position, clearanceRadius, obstacleLayer);

            bool isBlockedByCar = false;
            foreach (var hit in hits)
            {
                if (hit.GetComponent<CarAI>() != null || hit.GetComponentInParent<CarAI>() != null)
                {
                    isBlockedByCar = true;
                    break;
                }
            }

            if (!isBlockedByCar)
            {
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