using UnityEngine;
using System.Linq;

public class DroneController : MonoBehaviour
{
    [Header("DetectionRange")]
    public float detectionRange = 15f;
    public float chaseRange = 20f;
    public float attackRange = 5f;
    
    [Header("Behavior")]
    public float patrolCheckInterval = 0.5f;
    public float alertDuration = 3f;
    public float attackRate = 1f;
    public int attackDamage = 10;

    [Header("Movement")]
    public Transform[] waypoints;
    public float moveSpeed = 8f;
    public float rotationSpeed = 5f;
    public float hoverHeight = 4f;
    public float heightAdjustSpeed = 3f;

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
            Movement.waypoints = waypoints;
            Movement.moveSpeed = moveSpeed;
            Movement.rotationSpeed = rotationSpeed;
            Movement.hoverHeight = hoverHeight;
            Movement.heightAdjustSpeed = heightAdjustSpeed;
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
            .OrderBy(Tar => Vector3.Distance(transform.position, Tar.transform.position))
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
        if(Movement && Player)
        {
            if(Movement.enabled) Movement.enabled = false;
            
            transform.position = Vector3.MoveTowards(transform.position, Player.position, moveSpeed * Time.deltaTime);
            Vector3 directionToPlayer = (Player.position - transform.position).normalized;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
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