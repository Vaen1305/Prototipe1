using UnityEngine;
using System.Linq;

public class DroneController : MonoBehaviour
{
    [Header("Detection Ranges")]
    public float detectionRange = 15f;
    public float chaseRange = 20f;
    public float attackRange = 5f;
    
    [Header("Behavior Settings")]
    public float patrolCheckInterval = 0.5f;
    public float alertDuration = 3f;
    public float attackRate = 1f;
    public int attackDamage = 10;

    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float moveSpeed = 8f;
    public float rotationSpeed = 5f;
    public float hoverHeight = 4f;
    public float heightAdjustSpeed = 3f;

    [Header("Chase Settings")]
    public float chaseSmoothFactor = 3f; 

    public Transform Player { get; private set; }
    public Health PlayerHealth { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public MovementDrone Movement { get; private set; }
    
    private void Awake()
    {
        StateMachine = GetComponent<StateMachine>();
        Movement = GetComponent<MovementDrone>();
    }

    private void Start()
    {
        if (Movement)
        {
            Movement.SetWaypoints(waypoints);
            Movement.SetMovementStats(moveSpeed, rotationSpeed, hoverHeight, heightAdjustSpeed);
        }
    }

    public bool IsPlayerInChaseRange() => 
        Player && Vector3.Distance(transform.position, Player.position) <= chaseRange;
    
    public bool IsPlayerInAttackRange() => 
        Player && Vector3.Distance(transform.position, Player.position) <= attackRange;
    
    public void SetTarget(Transform newTarget)
    {
        Player = newTarget;
        if (Player != null)
        {
            PlayerHealth = Player.GetComponent<Health>();
        }
        else
        {
            PlayerHealth = null;
        }
    }

    public Transform FindBestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return null;

        Transform closestPlayer = players
            .OrderBy(p => Vector3.Distance(transform.position, p.transform.position))
            .FirstOrDefault()?.transform;

        if (closestPlayer && Vector3.Distance(transform.position, closestPlayer.position) > detectionRange)
        {
            return null;
        }
        return closestPlayer;
    }

    public void StartPatrolMovement()
    {
        if(Movement) Movement.enabled = true;
    }

    public void StopMovement()
    {
        if(Movement) Movement.enabled = false;
    }
    
    public void UpdateChase()
    {
        if (Player == null) return;

        if (Movement && Movement.enabled)
        {
            Movement.enabled = false;
        }
        
        transform.position = Vector3.Lerp(transform.position, Player.position, chaseSmoothFactor * Time.deltaTime);

        Vector3 directionToPlayer = (Player.position - transform.position).normalized;
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    public void PerformAttack()
    {
        if(PlayerHealth)
        {
            PlayerHealth.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}