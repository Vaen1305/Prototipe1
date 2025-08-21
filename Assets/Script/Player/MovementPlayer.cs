using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;

    [Header("Keys")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    private Rigidbody rb;
    private Vector3 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.freezeRotation = true; 
    }

    void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ProcessInput()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(forwardKey))
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(backwardKey))
        {
            moveDirection -= transform.forward;
        }
        if (Input.GetKey(leftKey))
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(rightKey))
        {
            moveDirection += transform.right;
        }

        if (moveDirection.sqrMagnitude > 1)
        {
            moveDirection.Normalize();
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}