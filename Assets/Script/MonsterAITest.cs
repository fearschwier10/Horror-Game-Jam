using UnityEngine;
using UnityEngine.AI;

public class MonsterAITest : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points
    public float detectionRange = 10f; // Range within which the monster detects the player
    public Transform player; // Reference to the player's transform

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
        currentPatrolIndex = 0;
        state = MonsterStates.Patroling;
        GoToNextPatrolPoint();
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
    public void TeleportMonster(Transform trans) => TeleportMonster(trans.position);
    public void TeleportMonster(Vector3 Position)
    {
        transform.position = Position;
    }

}

