using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MonsterAITest : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float detectionRange = 10f;
    public Transform player;
    public GameObject gameOverUI;

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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>(); // Initialize animator
        currentPatrolIndex = 0;
        state = MonsterStates.Patroling;
        GoToNextPatrolPoint();
        gameOverUI.SetActive(false);
        agent.isStopped = true; // Initially stop the monster
        animator.SetBool("isMoving", false); // Ensure the moving animation is off initially
        Debug.Log("Monster State: " + state);
    }

    void Update()
    {
        if (!isActive) // Check if the monster is not active
        {
            WatchPlayer(); // Watch the player while inactive
            animator.SetBool("isMoving", false); // Ensure idle animation is played
            return; // Skip the rest of the Update logic
        }

        if (state == MonsterStates.Chasing)
        {
            ChasePlayer();
            animator.SetBool("isMoving", true); // Set moving animation on
        }
        else if (state == MonsterStates.Patroling)
        {
            Patrol();
            CheckForPlayer();
            animator.SetBool("isMoving", true); // Set moving animation on
        }
        else if (state == MonsterStates.Idle)
        {
            WatchPlayer(); // Watch the player while idling
            animator.SetBool("isMoving", false); // Set moving animation off
        }

        Debug.Log("Monster State: " + state); // Log the current state each frame
    }

    void Patrol()
    {
        if (agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        Debug.Log("Moving to Patrol Point: " + currentPatrolIndex);
    }

    void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer < detectionRange)
        {
            state = MonsterStates.Chasing;
            PlayChaseSound();  // Play chase sound
            Debug.Log("Player detected! Switching to Chasing state.");
        }
    }

    void ChasePlayer()
    {
        agent.destination = player.position;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer > detectionRange)
        {
            state = MonsterStates.Patroling;
            GoToNextPatrolPoint();
            Debug.Log("Player out of range! Switching to Patrolling state.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Monster hit the player!");
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
        }
    }

    void PlayKillSound()
    {
        if (killSound != null)
        {
            audioSource.clip = killSound;
            audioSource.Play();
        }
    }

    void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Game Over!"); // Log Game Over
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
        animator.SetBool("isIdle", true); // Trigger the idle animation
        WatchPlayer(); // Optionally keep watching the player while inactive
        Debug.Log("Monster Deactivated. State: " + state);
    }

    // Teleport the monster and handle idle behavior based on toggle
    public void TeleportMonster(Transform trans) => TeleportMonster(trans.position);
    public void TeleportMonster(Vector3 position)
    {
        transform.position = position;
        agent.isStopped = true; // Stop the monster from moving

        if (shouldIdleOnTeleport)
        {
            // If the toggle is enabled, switch to idle behavior
            state = MonsterStates.Idle;
            animator.SetBool("isIdle", true); // Play the idle animation
            Debug.Log("Monster Teleported. State: " + state);
        }
        else
        {
            // If the toggle is disabled, continue patrolling or chasing
            agent.isStopped = false; // Allow movement again
            state = MonsterStates.Patroling; // Resume patrolling (or you can decide based on context)
            GoToNextPatrolPoint();
            Debug.Log("Monster Teleported and is now Patrolling. State: " + state);
        }
    }

    // Monster watches the player while idling
    void WatchPlayer()
    {
        // Rotate to face the player
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the rotation flat on the Y-axis
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
    }
}
