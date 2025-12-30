using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float speed = 8f;
    public float rotationSpeed = 10f;
    public bool destroyAtEnd = false; // Check this for spawning cars

    [Header("Detection Settings")]
    public float sensorDistance = 5f;
    public LayerMask obstacleLayer; // Set to 'Default' or 'Car' in Inspector
    public string stopZoneTag = "StopZone";

    private int currentWaypointIndex = 0;
    private bool isStoppedByLight = false;
    private GameObject currentBlockingCube;

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 1. RESTART LOGIC: Check if the blocking cube was disabled by Master
        if (isStoppedByLight)
        {
            if (currentBlockingCube == null || !currentBlockingCube.activeInHierarchy)
            {
                isStoppedByLight = false;
                currentBlockingCube = null;
            }
        }

        // 2. CAR DETECTION: Look ahead for other cars
        bool isBlockedByCar = Physics.Raycast(transform.position + transform.forward, transform.forward, sensorDistance, obstacleLayer);

        if (isStoppedByLight || isBlockedByCar) return;

        // 3. MOVEMENT & ROTATION
        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 4. WAYPOINT SWITCHING & DESPAWNING
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            if (currentWaypointIndex == waypoints.Length - 1)
            {
                if (destroyAtEnd) Destroy(gameObject);
                else currentWaypointIndex = 0; // Loop for circling cars
            }
            else
            {
                currentWaypointIndex++;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(stopZoneTag))
        {
            isStoppedByLight = true;
            currentBlockingCube = other.gameObject;
        }
    }

    void OnDrawGizmos() // Visual sensor in Scene View
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + transform.forward, transform.forward * sensorDistance);
    }
}