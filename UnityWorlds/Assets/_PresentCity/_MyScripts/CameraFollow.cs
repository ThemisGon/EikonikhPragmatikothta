using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 150f;
    public Vector3 offset = new Vector3(0, 2.0f, -4.5f);

    [Header("Collision & Smoothing")]
    public LayerMask collisionLayers;
    public float cameraRadius = 0.2f;
    public float smoothSpeed = 10f; // Increase for faster reaction, decrease for softer

    private float maxDistance;
    private float currentDistance;
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        maxDistance = offset.magnitude;
        currentDistance = maxDistance;
    }

    void LateUpdate()
    {

        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            rotationY += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, -80f, 60f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        Vector3 direction = rotation * offset.normalized;
        float targetDistance = maxDistance;

        RaycastHit hit;
        if (Physics.SphereCast(player.position + Vector3.up * 1.5f, cameraRadius, direction, out hit, maxDistance, collisionLayers))
        {
            targetDistance = hit.distance;
        }

        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * smoothSpeed);

        transform.position = (player.position + Vector3.up * 1.5f) + direction * currentDistance;

        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}