using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VM_yellow : MonoBehaviour
{
    [Header("Path root must contain children named P1, P2, P3, P4")]
    public Transform pathRoot;

    [Header("Movement (XZ)")]
    public float speedNormal = 6f;
    public float speedSlow = 3f;           // P4 -> P1
    public float arriveDistanceXZ = 0.8f;

    [Header("Rotation")]
    public float turnSpeedDegPerSec = 200f;
    public bool keepUpright = true;

    [Header("Model Forward Fix")]
    public float modelForwardFixDegrees = 0f;

    [Header("Flying Height")]
    public float hoverOffset = 1.5f;
    public float climbSpeed = 1.5f;
    public float takeoffSeconds = 1.5f;

    [Header("Obstacle Avoidance (only when needed)")]
    public bool enableAvoidance = true;
    public LayerMask obstacleMask = ~0;
    public float avoidDistance = 5f;
    public float rayHeight = 0.5f;

    [Tooltip("How strong the lateral sidestep is when avoiding")]
    public float sideStepSpeed = 3f;

    [Tooltip("Lock left/right choice for this long once obstacle is detected")]
    public float avoidLockSeconds = 0.35f;

    Rigidbody rb;
    Transform[] points;
    int currentIndex = 0;
    float currentHoverOffset;

    // avoidance state
    int avoidSide = 0; // -1 left, +1 right, 0 none
    float avoidLockTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        if (pathRoot == null)
        {
            Debug.LogError("VM_yellow: pathRoot is not assigned.");
            enabled = false;
            return;
        }

        Transform p1 = pathRoot.Find("P1");
        Transform p2 = pathRoot.Find("P2");
        Transform p3 = pathRoot.Find("P3");
        Transform p4 = pathRoot.Find("P4");

        if (!p1 || !p2 || !p3 || !p4)
        {
            Debug.LogError("VM_yellow: CarPath must contain direct children named P1, P2, P3, P4.");
            enabled = false;
            return;
        }

        points = new Transform[4] { p1, p2, p3, p4 };

        rb.position = points[0].position;
        currentIndex = 0;
        currentHoverOffset = 0f;
        avoidSide = 0;
        avoidLockTimer = 0f;
    }

    void FixedUpdate()
    {
        if (points == null) return;

        // Smooth takeoff (ramp hover offset)
        if (takeoffSeconds > 0f)
        {
            float step = (hoverOffset / takeoffSeconds) * Time.fixedDeltaTime;
            currentHoverOffset = Mathf.MoveTowards(currentHoverOffset, hoverOffset, step);
        }
        else
        {
            currentHoverOffset = hoverOffset;
        }

        int nextIndex = (currentIndex + 1) % 4;
        Vector3 pt = points[nextIndex].position;

        // Desired Y (hover)
        float desiredY = pt.y + currentHoverOffset;

        // --- DIRECT movement target (always toward the point) ---
        Vector3 targetXZ = new Vector3(pt.x, rb.position.y, pt.z);
        Vector3 toTarget = targetXZ - rb.position;
        Vector3 targetDir = new Vector3(toTarget.x, 0f, toTarget.z);
        if (targetDir.sqrMagnitude < 0.0001f) targetDir = Vector3.forward;
        targetDir = targetDir.normalized;

        // --- Avoidance only if obstacle in FRONT ---
        Vector3 side = Vector3.zero;
        bool frontBlocked = false;

        if (enableAvoidance)
        {
            frontBlocked = IsFrontBlocked(targetDir);

            // countdown lock timer
            if (avoidLockTimer > 0f) avoidLockTimer -= Time.fixedDeltaTime;

            if (!frontBlocked)
            {
                // if path is clear -> do NOT steer, do NOT zig-zag
                avoidSide = 0;
            }
            else
            {
                // obstacle in front -> decide side (and lock)
                if (avoidSide == 0 || avoidLockTimer <= 0f)
                {
                    avoidSide = ChooseFreerSide(targetDir); // -1 or +1
                    avoidLockTimer = avoidLockSeconds;
                }

                // create a pure sideways vector
                Vector3 right = new Vector3(targetDir.z, 0f, -targetDir.x).normalized;
                side = (avoidSide == +1) ? right : -right;
            }
        }

        // --- Rotation: look where we're going (direct dir, or slightly offset if avoiding) ---
        Vector3 lookDir = (frontBlocked && side.sqrMagnitude > 0.0001f)
            ? (targetDir + side * 0.7f).normalized
            : targetDir;

        Quaternion look = Quaternion.LookRotation(lookDir, Vector3.up);
        if (Mathf.Abs(modelForwardFixDegrees) > 0.001f)
            look = look * Quaternion.Euler(0f, modelForwardFixDegrees, 0f);

        Quaternion newRot = Quaternion.RotateTowards(rb.rotation, look, turnSpeedDegPerSec * Time.fixedDeltaTime);
        if (keepUpright) newRot = Quaternion.Euler(0f, newRot.eulerAngles.y, 0f);
        rb.MoveRotation(newRot);

        // --- Move: ALWAYS go straight to point + optional sideways sidestep only when blocked ---
        float speed = GetSpeedForLeg(currentIndex, nextIndex);

        Vector3 newPos = Vector3.MoveTowards(rb.position, targetXZ, speed * Time.fixedDeltaTime);

        if (frontBlocked && side.sqrMagnitude > 0.0001f)
            newPos += side * (sideStepSpeed * Time.fixedDeltaTime);

        // Smooth Y
        newPos.y = Mathf.MoveTowards(rb.position.y, desiredY, climbSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);

        // Arrival check XZ
        float xzDist = Vector2.Distance(new Vector2(rb.position.x, rb.position.z), new Vector2(pt.x, pt.z));
        if (xzDist <= arriveDistanceXZ)
            currentIndex = nextIndex;
    }

    bool IsFrontBlocked(Vector3 forwardDir)
    {
        Vector3 origin = rb.position + Vector3.up * rayHeight;

        bool hit = Physics.Raycast(origin, forwardDir, out RaycastHit h, avoidDistance, obstacleMask);

        // Debug ray
        Debug.DrawRay(origin, forwardDir * avoidDistance, hit ? Color.red : Color.green);

        return hit;
    }

    int ChooseFreerSide(Vector3 forwardDir)
    {
        Vector3 origin = rb.position + Vector3.up * rayHeight;

        Vector3 right = new Vector3(forwardDir.z, 0f, -forwardDir.x).normalized;
        Vector3 left = -right;

        bool hitL = Physics.Raycast(origin, (forwardDir + left * 0.6f).normalized, out RaycastHit hL, avoidDistance, obstacleMask);
        bool hitR = Physics.Raycast(origin, (forwardDir + right * 0.6f).normalized, out RaycastHit hR, avoidDistance, obstacleMask);

        Debug.DrawRay(origin, (forwardDir + left * 0.6f).normalized * avoidDistance, hitL ? Color.red : Color.green);
        Debug.DrawRay(origin, (forwardDir + right * 0.6f).normalized * avoidDistance, hitR ? Color.red : Color.green);

        float leftClear = hitL ? hL.distance : avoidDistance;
        float rightClear = hitR ? hR.distance : avoidDistance;

        return (rightClear > leftClear) ? +1 : -1;
    }

    float GetSpeedForLeg(int fromIndex, int toIndex)
    {
        if (fromIndex == 3 && toIndex == 0) return speedSlow;
        return speedNormal;
    }
}
