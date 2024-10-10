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
    }

    void Update()
    {
        if (state == MonsterStates.Chasing)
        {
            ChasePlayer();
        }
        else if (state == MonsterStates.Patroling)
        {
            Patrol();
            CheckForPlayer();
        }
        else if (state == MonsterStates.Idle)
        {
            WatchPlayer(); // Watch the player while idling
        }
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
    }

    void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer < detectionRange)
        {
            state = MonsterStates.Chasing;
            PlayChaseSound();  // Play chase sound
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
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        }
        else
        {
            // If the toggle is disabled, continue patrolling or chasing
            agent.isStopped = false; // Allow movement again
            state = MonsterStates.Patroling; // Resume patrolling (or you can decide based on context)
            GoToNextPatrolPoint();
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

        // Optionally, do other idle behavior here
    }
}

