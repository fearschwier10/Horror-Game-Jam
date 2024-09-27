using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float speed = 5f;  // Speed variable for movement
    public float gravity = -9.81f;  // Gravity force
    public float groundCheckDistance = 0.1f;  // Distance to check if grounded

    private CharacterController characterController;  // Reference to the Character Controller
    private Camera mainCamera;  // Reference to the main camera
    private Vector3 movementVelocity;  // Store the movement velocity
    private bool isGrounded;

    void Start()
    {
        // Get the Character Controller component attached to the player
        characterController = GetComponent<CharacterController>();

        // Get the main camera
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = characterController.isGrounded;

        if (isGrounded && movementVelocity.y < 0)
        {
            movementVelocity.y = 0f;  // Reset the vertical velocity when grounded
        }

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

        // Apply movement
        characterController.Move(movement * Time.deltaTime);

        // Apply gravity
        movementVelocity.y += gravity * Time.deltaTime;
        characterController.Move(movementVelocity * Time.deltaTime);
    }
}
