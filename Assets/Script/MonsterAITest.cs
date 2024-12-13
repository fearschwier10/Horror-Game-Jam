using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class MonsterAITest : MonoBehaviour
{
    public EvidenceTracker evidenceTracker; // Reference to the EvidenceTracker
    public bool mainMonster = false; // Boolean to check if this is the main monster
    public float defaultSpeed = 3.5f;  // Monster's default speed
    public float chaseSpeed = 6f;      // Monster's increased speed for chasing
    public MonoBehaviour dialogueTrigger; // Drag your specific dialogue trigger here

    // Post-Processing effects for Game Over
    public PostProcessVolume postProcessVolume;  // Reference to the PostProcessVolume
    private Vignette vignette;                   // Vignette effect
    private ChromaticAberration chromaticAberration; // Chromatic Aberration effect
    private Grain grain;

    public Transform player;  // Reference to the player
    public GameObject gameOverUI;
    public Transform mouthPosition;  // The target position inside the monster's mouth
    public Camera mainCamera;        // Reference to the main camera
    public float cameraMoveSpeed = 2f; // Speed at which the camera moves to the monster's mouth

    // Audio Clips
    public AudioClip chaseSound;  // Sound when chasing
    public AudioClip killSound;   // Sound when killing the player
    public AudioClip roarSound;   // Sound for roaring
    private AudioSource chaseAudioSource;  // Audio source for chase sound
    private AudioSource roarAudioSource;   // Audio source for roar sound
    private AudioSource killAudioSource;   // Audio source for kill sound

    private NavMeshAgent agent;
    public Animator animator; // Reference to the Animator component

    private bool isActive = false; // Flag to control monster's activity
    private bool hasRoared = false; // Flag to track if the monster has already roared

    public enum MonsterStates
    {
        Chasing = 1,
        KillPlayer = 2,
        Idle = 3, // Idle state
    }

    public MonsterStates state;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        chaseAudioSource = gameObject.AddComponent<AudioSource>();
        roarAudioSource = gameObject.AddComponent<AudioSource>();
        killAudioSource = gameObject.AddComponent<AudioSource>();
        animator = GetComponent<Animator>(); // Initialize animator
    }

    void Start()
    {
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
            animator.SetBool("isMoving", false); // Ensure idle animation is played
            WatchPlayer(); // Ensure it watches the player when idle
            return; // Skip the rest of the Update logic
        }

        if (!mainMonster) // Check if this is not the main monster
        {
            animator.SetBool("isMoving", false); // Ensure it stays idle
            WatchPlayer(); // Make sure non-main monsters still watch the player
            return; // Skip the rest of the Update logic for non-main monsters
        }

        Debug.Log("Monster Active. Current State: " + state); // Log monster state each frame if active

        if (state == MonsterStates.Chasing)
        {
            ChasePlayer();
            animator.SetBool("isMoving", true); // Set moving animation on
        }
        else if (state == MonsterStates.Idle)
        {
            WatchPlayer(); // Watch the player while idling
            animator.SetBool("isMoving", false); // Set moving animation off
        }
    }

    void ChasePlayer()
    {
        if (!isActive) return; // Ensure the monster is active before chasing

        // Calculate the distance between the monster and the player
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // Start the chase sound if not already playing
        if (distanceToPlayer > 20f && !chaseAudioSource.isPlaying && chaseSound != null)
        {
            chaseAudioSource.clip = chaseSound;
            chaseAudioSource.loop = true; // Ensure the chase sound loops
            chaseAudioSource.Play();
        }

        // Roar when the monster is within 20 units of the player (and roar hasn't been triggered yet)
        if (distanceToPlayer <= 20f && !hasRoared && roarSound != null)
        {
            roarAudioSource.clip = roarSound;
            roarAudioSource.loop = false; // Roar sound plays once
            roarAudioSource.Play();
            hasRoared = true; // Set the flag to prevent multiple roars
        }

        // Adjust the speed based on the distance to the player
        chaseSpeed = Mathf.Lerp(3f, 10f, distanceToPlayer / 20f); // Adjust speed range as needed

        // Always update the monster's destination to the player's current position
        if (agent.isActiveAndEnabled) // Ensure the NavMeshAgent is active
        {
            agent.SetDestination(player.position); // Set the destination to the player's current position
            agent.isStopped = false;  // Ensure the agent is not stopped during the chase

            // Dynamically adjust the speed based on the distance to the player
            agent.speed = chaseSpeed; // Set the agent's speed based on the distance
        }

        // Log the distance to the player and target position
        Debug.Log("Chasing player to position: " + player.position + " | Distance to player: " + distanceToPlayer);
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
        if (chaseSound != null && !chaseAudioSource.isPlaying)
        {
            chaseAudioSource.clip = chaseSound;
            chaseAudioSource.Play();
            Debug.Log("Playing chase sound."); // Debug chase sound
        }
    }

    void PlayKillSound()
    {
        if (killSound != null)
        {
            killAudioSource.clip = killSound;
            killAudioSource.Play();
            Debug.Log("Playing kill sound."); // Debug kill sound
        }
    }

    void GameOver()
    {
        chaseAudioSource.Stop();
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

        // Hide evidence UI when game over is triggered
        if (EvidenceTracker.Instance != null)
        {
            EvidenceTracker.Instance.HideEvidenceUI(); // Hide the evidence UI
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

        // Reset the game over UI
        gameOverUI.SetActive(false);
    }

    public void ActivateMonster()
    {
        if (!mainMonster) return; // Only activate if this is the main monster

        // Check if all evidence has been collected before activating the monster
        if (EvidenceTracker.Instance.GetCollectedEvidenceCount() != EvidenceTracker.Instance.evidenceList.Count)
        {
            Debug.Log("Not all evidence collected yet.");
            return; // Don't activate monster if evidence is incomplete
        }

        isActive = true;
        state = MonsterStates.Chasing;  // Set state to chasing
    }

    public void DeactivateMonster()
    {
        isActive = false;
        state = MonsterStates.Idle; // Set state to idle
    }

    public void WatchPlayer()
    {
        // Make sure the monster watches the player when idle
        transform.LookAt(player);
    }

    public void TeleportMonster(Transform trans) => TeleportMonster(trans.position);

    public void TeleportMonster(Vector3 position)
    {
        if (!mainMonster || isActive) return; // Don't teleport if the monster is active

        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        // Find the nearest valid position on the NavMesh
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            // Move the monster to the valid NavMesh position
            transform.position = hit.position;
            agent.Warp(hit.position);

            // Set monster state to Idle and ensure it watches the player
            state = MonsterStates.Idle;  // Ensure it is in the idle state
            animator.SetBool("isMoving", false);  // Stop any movement animation

            // Have the monster continue watching the player
            WatchPlayer();

            Debug.Log("Monster teleported to position: " + hit.position); // Debug teleportation
        }
        else
        {
            // If no valid position is found, log a warning
            Debug.LogWarning("Teleport destination is not on the NavMesh.");
        }
    }
    public void StopChaseMusic()
    {
        if (chaseAudioSource != null && chaseAudioSource.isPlaying)
        {
            chaseAudioSource.Stop();
            Debug.Log("Chase music stopped.");
        }
    }
}



   
