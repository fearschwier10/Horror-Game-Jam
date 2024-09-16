using UnityEngine;
using UnityEngine.AI;

public class MonsterAITest : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points
    public float detectionRange = 10f; // Range within which the monster detects the player
    public Transform player; // Reference to the player's transform

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private bool isChasing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPatrolIndex = 0;
        isChasing = false;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
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
            isChasing = true;
        }
    }

    void ChasePlayer()
    {
        agent.destination = player.position;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer > detectionRange)
        {
            isChasing = false;
            GoToNextPatrolPoint();
        }
    }
}

