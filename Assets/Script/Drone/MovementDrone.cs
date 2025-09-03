using UnityEngine;

public class MovementDrone : MonoBehaviour
{
    [Header("Movement Modes")]
    public bool useFixedAltitude = true; 

    private float moveSpeed;
    private float rotationSpeed;
    private float hoverHeight;
    private float heightAdjustSpeed;

    [Header("Waypoint Behavior")]
    public bool useRandomWaypoints = true;
    public float waypointReachDistance = 1.5f;

    private Transform[] waypoints;
    private int currentWaypoint = 0;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private Vector3 targetDirection;

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        if (waypoints != null && waypoints.Length > 0)
        {
            currentWaypoint = 0;
            UpdateTargetPosition();
        }
    }

    public void SetMovementStats(float newMoveSpeed, float newRotationSpeed, float newHoverHeight, float newHeightAdjustSpeed)
    {
        moveSpeed = newMoveSpeed;
        rotationSpeed = newRotationSpeed;
        hoverHeight = newHoverHeight;
        heightAdjustSpeed = newHeightAdjustSpeed;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 2f;
        rb.angularDrag = 4f;

        if (waypoints != null && waypoints.Length > 0)
        {
            if (useRandomWaypoints)
            {
                currentWaypoint = Random.Range(0, waypoints.Length);
            }
            UpdateTargetPosition();
        }
        else
        {
            targetPosition = transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        UpdateWaypointLogic();
        MoveDrone();

        if (useFixedAltitude)
        {
            AdjustHeight();
        }
    }

    private void UpdateWaypointLogic()
    {
        Vector3 positionOnPlane = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetOnPlane = new Vector3(targetPosition.x, 0, targetPosition.z);

        if (Vector3.Distance(positionOnPlane, targetOnPlane) < waypointReachDistance)
        {
            if (useRandomWaypoints)
            {
                SelectRandomWaypoint();
            }
            else
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            }
            UpdateTargetPosition();
        }

        targetDirection = (targetPosition - transform.position).normalized;
    }

    private void UpdateTargetPosition()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        
        Transform currentWaypointTransform = waypoints[currentWaypoint];

        if (useFixedAltitude)
        {
            targetPosition = new Vector3(currentWaypointTransform.position.x, hoverHeight, currentWaypointTransform.position.z);
        }
        else
        {
            targetPosition = currentWaypointTransform.position;
        }
    }

    private void SelectRandomWaypoint()
    {
        if (waypoints.Length <= 1) return;
        int newWaypoint;
        do { newWaypoint = Random.Range(0, waypoints.Length); }
        while (newWaypoint == currentWaypoint);
        currentWaypoint = newWaypoint;
    }

    private void MoveDrone()
    {
        Vector3 force = targetDirection * moveSpeed;

        if (useFixedAltitude)
        {
            force.y = 0;
        }
        
        rb.AddForce(force);

        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    
    private void AdjustHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            float distanceToGround = hit.distance;
            float heightError = hoverHeight - distanceToGround;
            
            float verticalForce = heightError * heightAdjustSpeed;
            rb.AddForce(Vector3.up * verticalForce);
        }
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    public void SetRandomMovement(bool random)
    {
        useRandomWaypoints = random;
    }
}