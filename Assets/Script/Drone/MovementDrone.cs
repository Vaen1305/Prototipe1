using UnityEngine;

public class MovementDrone : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    public float moveSpeed = 8f;
    public float rotationSpeed = 5f;
    public float hoverHeight = 4f;
    public float heightAdjustSpeed = 3f;

    public bool useRandomWaypoints = true;
    public float waypointReachDistance = 1.5f;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private Vector3 targetDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 2f;
        rb.angularDrag = 4f;

        if (waypoints.Length > 0)
        {
            if (useRandomWaypoints)
            {
                currentWaypoint = Random.Range(0, waypoints.Length);
            }
            targetPosition = GetWaypointPosition(waypoints[currentWaypoint]);
        }
        else
        {
            targetPosition = transform.position + Vector3.forward * 5f;
        }
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        MoveDrone();
        AdjustHeight();
    }

    private void UpdateTargetDirection()
    {
        if (waypoints.Length > 0)
        {
            if (Vector3.Distance(transform.position, targetPosition) < waypointReachDistance)
            {
                if (useRandomWaypoints)
                {
                    SelectRandomWaypoint();
                }
                else
                {
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                }

                targetPosition = GetWaypointPosition(waypoints[currentWaypoint]);
            }
        }

        targetDirection = (targetPosition - transform.position).normalized;
    }

    private void SelectRandomWaypoint()
    {
        if (waypoints.Length <= 1) return;

        int newWaypoint;
        do
        {
            newWaypoint = Random.Range(0, waypoints.Length);
        }
        while (newWaypoint == currentWaypoint);

        currentWaypoint = newWaypoint;
    }

    private void MoveDrone()
    {
        Vector3 horizontalForce = targetDirection * moveSpeed;
        horizontalForce.y = 0;
        rb.AddForce(horizontalForce);

        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void AdjustHeight()
    {
        float heightDifference = hoverHeight - transform.position.y;

        if (Mathf.Abs(heightDifference) > 0.1f)
        {
            float verticalForce = Mathf.Sign(heightDifference) * heightAdjustSpeed;
            rb.AddForce(Vector3.up * verticalForce);
        }
    }

    private Vector3 GetWaypointPosition(Transform waypoint)
    {
        return new Vector3(waypoint.position.x, hoverHeight, waypoint.position.z);
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = new Vector3(newPosition.x, hoverHeight, newPosition.z);
    }

    public void SetRandomMovement(bool random)
    {
        useRandomWaypoints = random;
    }
}