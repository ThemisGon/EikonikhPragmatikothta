using UnityEngine;

public class CarAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    private int currentWaypointIndex = 0;
    private bool isStopped = false;

    void Update()
    {
        if (isStopped) return;

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.LookAt(target.position);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Detect the Stop Zone from the Master Controller
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StopZone")) isStopped = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StopZone")) isStopped = false;
    }
}