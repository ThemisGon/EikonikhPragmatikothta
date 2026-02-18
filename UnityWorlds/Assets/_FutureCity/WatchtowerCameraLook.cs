using UnityEngine;

public class WatchtowerCameraLook : MonoBehaviour
{
    public float sensitivity = 2.5f;
    public float minPitch = -40f;
    public float maxPitch = 70f;

    private float yaw;
    private float pitch;

    void OnEnable()
    {
        // Store current rotation as starting point
        Vector3 euler = transform.rotation.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
    }

    void Update()
    {
        // Rotate camera only while holding Right Mouse Button
        if (!Input.GetMouseButton(1)) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * sensitivity;
        pitch -= mouseY * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}