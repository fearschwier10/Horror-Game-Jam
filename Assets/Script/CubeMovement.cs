using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float speed = 5f;  // Speed variable for movement
    private Rigidbody rb;
    private Camera mainCamera;  // Reference to the main camera

    void Start()
    {
        // Get the Rigidbody component attached to the cube
        rb = GetComponent<Rigidbody>();

        // Ensure the Rigidbody is set to not use gravity and to use continuous collision detection
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Get the main camera
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Get input for horizontal and vertical movement
        float horizontal = Input.GetAxis("Horizontal");  // Left/Right or A/D
        float vertical = Input.GetAxis("Vertical");      // Up/Down or W/S

        // Calculate the movement direction relative to the camera
        Vector3 right = mainCamera.transform.right;
        Vector3 forward = mainCamera.transform.forward;

        // Flatten the direction vectors to ignore vertical movement
        right.y = 0;
        forward.y = 0;

        // Calculate the movement direction
        Vector3 movement = (right * horizontal + forward * vertical).normalized * speed;

        // Move the cube using Rigidbody
        rb.velocity = movement;
    }
}
