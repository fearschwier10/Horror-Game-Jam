using UnityEngine;
using UnityEngine.UI;  // For UI components

public class CubeMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeedMultiplier = 2f;
    public float sprintDuration = 3f;
    public float sprintCooldown = 5f;

    private CharacterController controller;
    private float speed;
    private bool isSprinting;
    private float sprintTimer;
    private float cooldownTimer;

    // UI Element
    public Slider sprintSlider;  // Reference to the UI Slider

    void Start()
    {
        controller = GetComponent<CharacterController>();
        speed = walkSpeed;
        isSprinting = false;
        sprintTimer = 0f;
        cooldownTimer = 0f;

        if (sprintSlider != null)
        {
            sprintSlider.maxValue = sprintDuration; // Set slider max to sprint duration
            sprintSlider.value = sprintDuration; // Initialize slider to full
        }
    }

    void Update()
    {
        HandleSprint();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            controller.Move(move * speed * Time.deltaTime);
        }
    }

    void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isSprinting && cooldownTimer <= 0f)
        {
            StartSprint();
        }

        // Check if sprint key is released while sprinting
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
            UpdateSprintUI(); // Update UI during cooldown as well
        }
    }

    void StartSprint()
    {
        isSprinting = true;
        speed = walkSpeed * sprintSpeedMultiplier;
        sprintTimer = sprintDuration;
        UpdateSprintUI(); // Update UI on sprint start
    }

    void StopSprint()
    {
        isSprinting = false;
        speed = walkSpeed;
        cooldownTimer = sprintCooldown; // Start cooldown only if sprint duration is over
        UpdateSprintUI(); // Update UI on sprint stop
    }

    void UpdateSprintUI()
    {
        if (sprintSlider != null)
        {
            sprintSlider.value = isSprinting ? sprintTimer : Mathf.Max(0, sprintCooldown - cooldownTimer);
            sprintSlider.maxValue = isSprinting ? sprintDuration : sprintCooldown;
        }
    }
}
