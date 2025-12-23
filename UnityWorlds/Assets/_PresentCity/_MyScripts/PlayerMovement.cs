using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Animator anim; // This connects to the "Brain"

    public float speed = 6f;
    public float jumpHeight = 2.0f;
    public float gravity = -20f;

    private Vector3 velocity;
    private bool isGrounded;

    void Update()
    {
        // 1. Gravity Check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // 2. Get Keyboard Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 3. Move & Rotate
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // ---> TELL ANIMATOR TO WALK <---
            // We use the exact name "isWalking" you created in the Animator
            if (anim != null) anim.SetBool("isWalking", true);
        }
        else
        {
            // ---> TELL ANIMATOR TO STOP <---
            if (anim != null) anim.SetBool("isWalking", false);
        }

        // 4. Jump Logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            // ---> TELL ANIMATOR TO JUMP <---
            // We use the exact name "jump" you created in the Animator
            if (anim != null) anim.SetTrigger("jump");
        }

        // 5. Apply Physics
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}