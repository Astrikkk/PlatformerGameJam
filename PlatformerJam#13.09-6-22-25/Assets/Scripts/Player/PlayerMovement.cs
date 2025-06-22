using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground check parameters")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private float ceilingCheckDistance = 8f;

    [Header("Rotation")]
    [SerializeField] private float rotationDuration = 0.5f;

    private Rigidbody2D rb;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool ceilingExists = false;
    private float horizontalInput;
    private bool isRotating = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isRotating) return;

        CheckGrounded();
        CheckCeiling();
        if (isGrounded)
        {
            if (horizontalInput != 0)
            {
                float targetSpeed = horizontalInput * maxSpeed;
                float speedDiff = targetSpeed - rb.velocity.x;
                float accelerationRate = (speedDiff > 0) ? acceleration : deceleration;
                float newXVelocity = rb.velocity.x + speedDiff * accelerationRate * Time.fixedDeltaTime;

                newXVelocity = Mathf.Clamp(newXVelocity, -maxSpeed, maxSpeed);
                rb.velocity = new Vector2(newXVelocity, rb.velocity.y);
            }
            else
            {
                float newXVelocity = Mathf.Lerp(rb.velocity.x, 0f, deceleration * Time.fixedDeltaTime);
                rb.velocity = new Vector2(newXVelocity, rb.velocity.y);
            }
        }
    }

    private void Update()
    {
        if (isGrounded)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.E) && ceilingExists && !isRotating && isGrounded) 
        {
            StartCoroutine(ChangeDirection());
        }
    }

    private void Jump()
    {
        float speedBoost = Mathf.Abs(rb.velocity.x) / 2f;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce + speedBoost);
    }

    private IEnumerator ChangeDirection()
    {
        float speedBoost = Mathf.Abs(rb.velocity.x) / 2f;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce * rb.gravityScale);

        isRotating = true;
        rb.gravityScale *= -1;

        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, 180);

        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }

    private void CheckGrounded()
    {
        Vector2 rayDirection = (rb.gravityScale > 0) ? Vector2.down : Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void CheckCeiling()
    {
        Vector2 rayDirection = (rb.gravityScale > 0) ? Vector2.up : Vector2.down;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, ceilingCheckDistance, groundLayer);
        ceilingExists = hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 rayDirection = (Application.isPlaying && rb != null && rb.gravityScale > 0) ? Vector2.down : Vector2.up;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rayDirection * groundCheckDistance);

        Gizmos.color = Color.blue;
        Vector2 ceilingDirection = (Application.isPlaying && rb != null && rb.gravityScale > 0) ? Vector2.up : Vector2.down;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)ceilingDirection * ceilingCheckDistance);
    }
}