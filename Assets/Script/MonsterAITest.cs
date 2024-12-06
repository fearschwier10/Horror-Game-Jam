using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;


public class MonsterAITest : MonoBehaviour

{
    // Add at the top of the script
    public float defaultSpeed = 3.5f;  // Monster's default speed
    public float chaseSpeed = 6f;      // Monster's increased speed for chasing
    public MonoBehaviour dialogueTrigger; // Drag your specific dialogue trigger here

    // Post-Processing effects for Game Over
    public PostProcessVolume postProcessVolume;  // Reference to the PostProcessVolume
    private Vignette vignette;                   // Vignette effect
    private ChromaticAberration chromaticAberration; // Chromatic Aberration effect
    private Grain grain;

    public Transform[] patrolPoints;
    public float detectionRange = 10f;
    public Transform player;
    public GameObject gameOverUI;
    public Transform mouthPosition;  // The target position inside the monster's mouth
    public Camera mainCamera;        // Reference to the main camera
    public float cameraMoveSpeed = 2f; // Speed at which the camera moves to the monster's mouth


    // Audio Clips
    public AudioClip chaseSound;  // Sound when chasing
    public AudioClip killSound;   // Sound when killing the player
    private AudioSource audioSource;  // Reference to the AudioSource component

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    public Animator animator; // Reference to the Animator component

    public bool shouldIdleOnTeleport = false; // Toggle to control idle behavior after teleport
    private bool isActive = false; // Flag to control monster's activity

    public enum MonsterStates
    {
        Patroling = 0,
        Chasing = 1,
        KillPlayer = 2,
        Idle = 3, // Idle state
    }
    public MonsterStates state;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>(); // Initialize animator
    }

    void Start()
    {
        currentPatrolIndex = 0;
        gameOverUI.SetActive(false);
        DeactivateMonster();
        Debug.Log("Monster State at Start: " + state);

        // Initialize post-processing effects
        if (postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGetSettings(out vignette);
            postProcessVolume.profile.TryGetSettings(out chromaticAberration);
            postProcessVolume.profile.TryGetSettings(out grain);
        }
    }

    void Update()
    {
        if (!isActive) // Check if the monster is not active
        {
            WatchPlayer(); // Watch the player while inactive
            animator.SetBool("isMoving", false); // Ensure idle animation is played
            Debug.Log("Monster is inactive and watching player."); // Debug message for inactive state
            return; // Skip the rest of the Update logic
        }

        Debug.Log("Monster Active. Current State: " + state); // Log monster state each frame if active
        Debug.Log("Monster moving agent is stopped: " + agent.isStopped);

        if (state == MonsterStates.Chasing)
        {
            Debug.Log("Monster is chasing the player."); // Debug message for chasing state
            ChasePlayer();
            animator.SetBool("isMoving", true); // Set moving animation on
        }
        else if (state == MonsterStates.Patroling)
        {
            Debug.Log("Monster is patrolling."); // Debug message for patrolling state
            Patrol();
            CheckForPlayer();  // Continue checking for the player while patrolling
            animator.SetBool("isMoving", true); // Set moving animation on
        }
        else if (state == MonsterStates.Idle)
        {
            Debug.Log("Monster is idle and watching player."); // Debug message for idle state
            WatchPlayer(); // Watch the player while idling
            animator.SetBool("isMoving", false); // Set moving animation off
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        // Set the destination to the next patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        agent.isStopped = false; // Allow movement again
        Debug.Log("Moving to Patrol Point: " + currentPatrolIndex + " at Position: " + agent.destination); // Debug patrol point details
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void Patrol()
    {
        // Check if the agent is actively calculating a path
        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            Debug.Log("Path is complete. Agent is on a valid path to the patrol point.");
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            Debug.LogWarning("Partial path detected. Agent may not reach the patrol point.");
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError("Invalid path! Agent cannot reach the patrol point.");
            return; // Exit the function if the path is invalid
        }

        // Continue to check remaining distance and move to the next patrol point
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            Debug.Log("Arrived at Patrol Point: " + currentPatrolIndex); // Debug message for arrival at patrol point
            GoToNextPatrolPoint();
        }
    }


    void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        Debug.Log("Distance to Player: " + distanceToPlayer); // Debug player distance

        if (distanceToPlayer < detectionRange)
        {
            state = MonsterStates.Chasing;
            PlayChaseSound();  // Play chase sound
            Debug.Log("Player detected! Switching to Chasing state."); // Debug player detection
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        Debug.Log("Chasing player to position: " + player.position); // Debug target player position

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer > detectionRange)
        {
            state = MonsterStates.Patroling;  // Switch back to patrolling if the player goes out of range
            GoToNextPatrolPoint();  // Return to patrolling at the next patrol point
            Debug.Log("Player out of range! Switching to Patrolling state."); // Debug player out of range
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Monster hit the player! Triggering Game Over."); // Debug player collision
            state = MonsterStates.KillPlayer;
            PlayKillSound();  // Play kill sound
            GameOver();
        }
    }

    void PlayChaseSound()
    {
        if (chaseSound != null)
        {
            audioSource.clip = chaseSound;
            audioSource.Play();
            Debug.Log("Playing chase sound."); // Debug chase sound
        }
    }

    void PlayKillSound()
    {
        if (killSound != null)
        {
            audioSource.clip = killSound;
            audioSource.Play();
            Debug.Log("Playing kill sound."); // Debug kill sound
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over triggered."); // Debug Game Over
        StartCoroutine(MoveCameraToMouth());

        // Start applying post-processing effects for Game Over
        if (vignette != null)
        {
            vignette.intensity.value = 1.0f;  // Example intensity for vignette effect
        }

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = 3.0f;  // Example intensity for chromatic aberration
        }

        if (grain != null)
        {
            grain.intensity.value = 6.0f;  // Example intensity for grain effect
        }
    }

    IEnumerator MoveCameraToMouth()
    {
        Debug.Log("Starting camera movement to mouth."); // Debug camera movement
        agent.isStopped = true; // Stop the monster's movement
        animator.SetBool("isMoving", false); // Stop monster animations

        // Store the original camera position and rotation
        Vector3 originalPosition = mainCamera.transform.position;
        Quaternion originalRotation = mainCamera.transform.rotation;

        // Define how much the camera will move backward before zooming in
        float backwardDistance = 5f;
        float zoomInDistance = 2f; // Distance the camera will move to zoom in
        float zoomTime = 0.5f; // Time for the zoom effect

        // Calculate the target position for zooming out (moving backward)
        Vector3 targetPosition = originalPosition - mainCamera.transform.forward * backwardDistance;

        float elapsedTime = 0f;

        // Move the camera backward to simulate zoom-out
        while (elapsedTime < zoomTime)
        {
            mainCamera.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / zoomTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Now move the camera smoothly to the monster's mouth
        elapsedTime = 0f;
        Vector3 mouthPosition = this.mouthPosition.position;

        // Move towards the monster's mouth (zoom in effect)
        while (elapsedTime < zoomTime)
        {
            mainCamera.transform.position = Vector3.Lerp(targetPosition, mouthPosition, elapsedTime / zoomTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera is exactly at the mouth position
        mainCamera.transform.position = mouthPosition;

        Debug.Log("Camera movement complete. Showing Game Over UI."); // Debug completion
        ShowGameOverUI(); // Show the Game Over screen
    }


    void ShowGameOverUI()
    {
        gameOverUI.SetActive(true); // Activate the Game Over screen
        Time.timeScale = 0f;        // Freeze game time
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void RestartGame()
    {
        Debug.Log("Restart Game button clicked!");
        Time.timeScale = 1f;  // Resume time
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload the current scene
    }

    // Method to activate the monster
    public void ActivateMonster()
    {
        isActive = true; // Set the monster to active
        agent.isStopped = false; // Allow movement
        state = MonsterStates.Patroling; // Start patrolling
        GoToNextPatrolPoint(); // Move to the next patrol point
        Debug.Log("Monster Activated. State: " + state);
    }

    public void DeactivateMonster()
    {
        isActive = false; // Set the monster to inactive
        agent.isStopped = true; // Stop any movement
        state = MonsterStates.Idle; // Switch to the idle state
        animator.SetBool("isMoving", false); // Ensure the moving animation is off
        Debug.Log("Monster Deactivated. State: " + state);
        WatchPlayer(); // Optionally keep watching the player while inactive
    }

    // Teleport the monster and handle idle behavior based on toggle
    public void TeleportMonster(Transform trans) => TeleportMonster(trans.position);
    public void TeleportMonster(Vector3 position)
    {
        transform.position = position;

        if (shouldIdleOnTeleport)
        {
            DeactivateMonster();
            Debug.Log("Monster Teleported. State: " + state);
        }
        else
        {
            // If the toggle is disabled, continue patrolling or chasing
            state = MonsterStates.Patroling; // Resume patrolling (or you can decide based on context)
            GoToNextPatrolPoint();
            Debug.Log("Monster Teleported and is now Patrolling. State: " + state);
        }
    }
    public void ForceChasePlayer()
    {
        state = MonsterStates.Chasing;  // Set the monster's state to chasing

        agent.isStopped = true;  // Stop the monster from patrolling
        agent.speed *= 2;  // Double the speed for chasing
        agent.SetDestination(player.position);  // Set the destination to the player's position

        PlayChaseSound();  // Play the chase sound (optional)
        Debug.Log("Monster forced to chase the player with doubled speed.");  // Debug message
    }
    // Monster watches the player while idling
    void WatchPlayer()
    {
        // Rotate to face the player
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the rotation flat on the Y-axis
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
        Debug.Log("Monster is watching the player."); // Debug watching player
    }
}