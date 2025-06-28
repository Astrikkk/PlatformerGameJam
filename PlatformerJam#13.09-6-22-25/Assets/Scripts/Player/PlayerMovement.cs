using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    [Header("Stamina Settings")]
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private int dashStaminaCost = 20;
    [SerializeField] private int jumpStaminaCost = 20;
    [SerializeField] private GameObject staminaBar;
    public int CurrentStamina;
    private bool canRegenStm;
    private float idleTimer = 0f;
    private bool wasMoving = false;
    [SerializeField] private float idleTimeThreshold = 2f;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 0.5f;
    private bool isInvincible = false;
    private Coroutine invincibilityCoroutine;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float horizontalInput;
    private bool isDashing = false;
    private SpriteRenderer spriteRenderer;
    private bool justJumped = false;
    [SerializeField] private camController _cm;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentStamina = maxStamina;
    }

    private void FixedUpdate()
    {
        staminaBar.GetComponent<Scrollbar>().size = CurrentStamina * 0.01f;
        if (CurrentStamina < maxStamina && canRegenStm) RegenerateStamina();

        if (!isDashing)
        {
            if (!justJumped) CheckGrounded();
            else Invoke("CheckGrounded", 0.5f);

            if (isGrounded)
            {
                HandleMovement();
            }
        }

        CheckIdleState();
    }

    private void CheckIdleState()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(rb.velocity.x) > 0.1f;

        if (isMoving)
        {
            idleTimer = 0f;
            wasMoving = true;
            canRegenStm = false;
        }
        else if (wasMoving)
        {
            idleTimer += Time.fixedDeltaTime;

            if (idleTimer >= idleTimeThreshold)
            {
                OnIdleTooLong();
                wasMoving = false;
                idleTimer = 0f;
            }
        }
    }

    private void OnIdleTooLong()
    {
        _cm.camToCenter();
        canRegenStm = true;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        horizontalInput = rb.gravityScale > 0 ?
            Input.GetAxisRaw("Horizontal") :
            -Input.GetAxisRaw("Horizontal");

        if (horizontalInput != 0)
        {
            _cm.transposer.m_ScreenX = horizontalInput > 0 ? _cm.rightOffset : _cm.leftOffset;
            spriteRenderer.flipX = horizontalInput < 0;
        }

        if (Input.GetKeyDown(KeyCode.Space)) TryJump();
        if (Input.GetKeyDown(KeyCode.LeftShift)) TryDash();
    }

    private void HandleMovement()
    {
        if (horizontalInput != 0)
        {
            float targetSpeed = horizontalInput * maxSpeed;
            float speedDiff = targetSpeed - rb.velocity.x;
            float accelerationRate = (speedDiff > 0) ? acceleration : deceleration;
            float newXVelocity = rb.velocity.x + speedDiff * accelerationRate * Time.fixedDeltaTime;
            rb.velocity = new Vector2(Mathf.Clamp(newXVelocity, -maxSpeed, maxSpeed), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0f, deceleration * Time.fixedDeltaTime), rb.velocity.y);
        }

        animator.SetBool("isWalking", Mathf.Abs(horizontalInput) > 0.1f);
    }

    private void TryJump()
    {
        if (isGrounded && CurrentStamina >= jumpStaminaCost)
        {
            animator.SetTrigger("Jump");
            Invoke("Jump", 0.3f);
        }
    }

    private void Jump()
    {
        justJumped = true;
        isGrounded = false;
        float speedBoost = Mathf.Abs(rb.velocity.x) / 2f;
        rb.velocity = new Vector2(rb.velocity.x, (jumpForce + speedBoost) * Mathf.Sign(rb.gravityScale));
        ModifyStamina(-jumpStaminaCost);
    }

    private void TryDash()
    {
        if (!isDashing && CurrentStamina >= dashStaminaCost && !isInvincible)
        {
            animator.SetTrigger("Dash");
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;
        ModifyStamina(-dashStaminaCost);

        float dashDirection = horizontalInput != 0 ? Mathf.Sign(horizontalInput) : spriteRenderer.flipX ? -1 : 1;
        rb.velocity = new Vector2(dashDirection * dashForce, 0f);

        if (invincibilityCoroutine != null) StopCoroutine(invincibilityCoroutine);
        invincibilityCoroutine = StartCoroutine(InvincibilityEffect());

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(invincibilityDuration - dashDuration);
        isInvincible = false;
    }

    public void InvertGravity()
    {
        justJumped = true;
        isGrounded = false;
        groundCheckDistance = -groundCheckDistance;
        animator.SetTrigger("Flip");
        Invoke("Invert", 1.1f);
    }

    private void Invert()
    {
        spriteRenderer.flipY = !spriteRenderer.flipY;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void Landed()
    {
        justJumped = false;
        animator.SetTrigger("Land");
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (!isGrounded) Landed();
    }

    private void RegenerateStamina()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 1f || Mathf.Abs(rb.velocity.x) > 1f;
        if (!isMoving && !isDashing && isGrounded)
        {
            ModifyStamina(Mathf.RoundToInt(staminaRegenRate * Time.fixedDeltaTime));
        }
    }

    public void ModifyStamina(int amount)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina + amount, 0, maxStamina);
    }

    private IEnumerator InvincibilityEffect()
    {
        while (isInvincible)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public bool IsInvincible() => isInvincible;
    public bool IsGrounded() => isGrounded;
    public bool IsDashing() => isDashing;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}