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
    public enum MonsterStates
    {
        Patroling = 0,
        Chasing = 1,
        KillPlayer = 2,
    }
    public MonsterStates state;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
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

    // Change to OnTriggerEnter
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
}
