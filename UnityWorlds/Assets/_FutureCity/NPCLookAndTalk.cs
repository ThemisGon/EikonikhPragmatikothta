using UnityEngine;

public class NPCLookAndTalk : MonoBehaviour
{
    [Header("Look At")]
    [SerializeField] Transform rotateRoot;   //
    [SerializeField] float turnSpeed = 8f;
    [SerializeField] bool onlyYaw = true;

    Transform player;
    bool active;

    Quaternion savedRotation;

    void Reset()
    {
        // auto-setup
        rotateRoot = transform;

    }

    void Awake()
    {
        if (rotateRoot == null) rotateRoot = transform;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (!active || player == null) return;

        Vector3 dir = player.position - rotateRoot.position;
        if (onlyYaw) dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
        rotateRoot.rotation = Quaternion.Slerp(rotateRoot.rotation, target, turnSpeed * Time.deltaTime);
    }

    public void StartFacing()
    {
        if (active) return;

        // save where NPC was looking
        savedRotation = rotateRoot.rotation;
        active = true;

    }

    public void StopFacing()
    {
        if (!active) return;

        active = false;

        // restore old rotation
        rotateRoot.rotation = savedRotation;
    }
}
