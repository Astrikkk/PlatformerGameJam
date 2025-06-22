using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ // ####################### IMPORTANT ############## player must be on 0,0 coordinates for proper camera movement, idk why
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground check parameters")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private Rigidbody2D rb;
    [SerializeField] private bool isGrounded = false;
    private float horizontalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
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
        if (isGrounded) {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            Jump();
        }
    }

    private void Jump()
    {
        float speedBoost = Mathf.Abs(rb.velocity.x) / 2f;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce + speedBoost);
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}