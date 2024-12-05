using UnityEngine;
using UnityEngine.UI;

public class CubeMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeedMultiplier = 2f;
    public float sprintDuration = 3f;
    public float sprintCooldown = 5f;
    public float gravity = -9.81f;   // Gravity strength
    public float groundCheckDistance = 0.2f; // Distance to check if on ground

    private CharacterController controller;
    private float speed;
    private bool isSprinting;
    private float sprintTimer;
    private float cooldownTimer;
    private float verticalVelocity;  // Tracks downward speed for gravity

    // UI Element
    public Slider sprintSlider;  // Reference to the UI Slider

    // Audio settings
    private AudioSource audioSource;  // Reference to the AudioSource component
    public AudioClip walkingClip;    // The walking sound clip

    void Start()
    {
        controller = GetComponent<CharacterController>();
        speed = walkSpeed;
        isSprinting = false;
        sprintTimer = 0f;
        cooldownTimer = 0f;
        verticalVelocity = 0f;

        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

        if (sprintSlider != null)
        {
            sprintSlider.maxValue = sprintDuration; // Set slider max to sprint duration
            sprintSlider.value = sprintDuration; // Initialize slider to full
        }
    }

    void Update()
    {
        HandleSprint();

        // Handle horizontal movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Apply gravity if not grounded
        if (controller.isGrounded)
        {
            verticalVelocity = 0f; // Reset vertical velocity if grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Final movement calculation
        Vector3 move = (transform.right * horizontal + transform.forward * vertical) * speed;
        move.y = verticalVelocity; // Apply gravity to the movement vector

        controller.Move(move * Time.deltaTime);

        // Play walking audio if moving
        if (move.magnitude > 0.1f) // Check if the character is moving
        {
            PlayWalkingAudio();
        }
        else
        {
            StopWalkingAudio();
        }
    }

    void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isSprinting && cooldownTimer <= 0f)
        {
            StartSprint();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && isSprinting)
        {
            StopSprint();
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0f)
            {
                StopSprint();
            }
            UpdateSprintUI();
        }
        else if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            UpdateSprintUI();
        }
    }

    void StartSprint()
    {
        isSprinting = true;
        speed = walkSpeed * sprintSpeedMultiplier;
        sprintTimer = sprintDuration;
        UpdateSprintUI();
    }

    void StopSprint()
    {
        isSprinting = false;
        speed = walkSpeed;
        cooldownTimer = sprintCooldown;
        UpdateSprintUI();
    }

    void UpdateSprintUI()
    {
        if (sprintSlider != null)
        {
            sprintSlider.value = isSprinting ? sprintTimer : Mathf.Max(0, sprintCooldown - cooldownTimer);
            sprintSlider.maxValue = isSprinting ? sprintDuration : sprintCooldown;
        }
    }

    // Method to play walking audio
    void PlayWalkingAudio()
    {
        if (!audioSource.isPlaying && walkingClip != null) // Check if audio is not already playing
        {
            audioSource.clip = walkingClip;
            audioSource.loop = true; // Loop the walking audio
            audioSource.Play();
        }
    }

    // Method to stop walking audio
    void StopWalkingAudio()
    {
        if (audioSource.isPlaying) // Check if audio is currently playing
        {
            audioSource.Stop();
        }
    }
}
