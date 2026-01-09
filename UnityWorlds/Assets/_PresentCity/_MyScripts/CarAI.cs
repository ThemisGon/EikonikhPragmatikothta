using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float speed = 8f;
    public float rotationSpeed = 10f;
    public bool destroyAtEnd = false;

    [Header("Detection Settings")]
    public float sensorDistance = 5f;
    public LayerMask obstacleLayer;
    public string stopZoneTag = "StopZone";

    private int currentWaypointIndex = 0;
    private bool isStoppedByLight = false;
    private GameObject currentBlockingCube;

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (isStoppedByLight)
        {
            if (currentBlockingCube == null || !currentBlockingCube.activeInHierarchy)
            {
                isStoppedByLight = false;
                currentBlockingCube = null;
            }
        }

        Vector3 sensorStartPos = transform.position + transform.forward + (Vector3.up * 1f);
        bool isBlockedByCar = Physics.Raycast(sensorStartPos, transform.forward, sensorDistance, obstacleLayer);

        if (isStoppedByLight || isBlockedByCar) return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            if (currentWaypointIndex == waypoints.Length - 1)
            {
                if (destroyAtEnd) Destroy(gameObject);
                else currentWaypointIndex = 0;
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 sensorStartPos = transform.position + transform.forward + (Vector3.up * 1f);
        Gizmos.DrawRay(sensorStartPos, transform.forward * sensorDistance);
    }
}