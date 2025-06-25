using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("Ground check parameters")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    [Header("Idle detection")]
    [SerializeField] private float idleTimeThreshold = 2f;

    [Header("Stamina settings")]
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private int dashStaminaCost = 20;
    [SerializeField] private int jumpStaminaCost = 20;
    [SerializeField] private GameObject staminaBar;
    public int CurrentStamina;
    private bool canRegenStm;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 0.5f;
    private bool isInvincible = false;
    private Coroutine invincibilityCoroutine;

    private Rigidbody2D rb;
    [SerializeField] private bool isGrounded = false;
    private float horizontalInput;
    private bool isDashing = false;
    private float idleTimer = 0f;
    private bool wasMoving = false;

    [SerializeField] private camController _cm;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentStamina = maxStamina;
    }

    private void FixedUpdate()
    {
        staminaBar.GetComponent<Scrollbar>().size = CurrentStamina * 0.01f;
        if(CurrentStamina < maxStamina && canRegenStm)
            RegenerateStamina();
        if(rb.gravityScale > 0)
            groundCheckDistance = 1.1f;
        else if(rb.gravityScale < 0)
            groundCheckDistance = -1.1f;   
         
        if (!isDashing)
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

        CheckIdleState();
    }

    private void Update()
    {
        if(rb.gravityScale > 0)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            if(horizontalInput > 0f)
            {
                _cm.transposer.m_ScreenX = _cm.rightOffset;
            }
            else if(horizontalInput < 0f)
            {
                _cm.transposer.m_ScreenX = _cm.leftOffset;
            }
        }
        else if(rb.gravityScale < 0)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal") * -1;
            if(horizontalInput > 0f)
            {
                _cm.transposer.m_ScreenX = _cm.rightOffset;
            }
            else if(horizontalInput < 0f)
            {
                _cm.transposer.m_ScreenX = _cm.leftOffset;
            }
        }
            
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && CurrentStamina >= jumpStaminaCost)
        {
            Jump();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash();
        }
    }


    public void ModifyStamina(int amount)
    {
        CurrentStamina += amount;
    }

    private void RegenerateStamina()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(rb.velocity.x) > 0.1f;
        if(!isMoving && !isDashing && isGrounded)
        {
            float regenAmount = staminaRegenRate * Time.fixedDeltaTime;
            ModifyStamina(Mathf.RoundToInt(regenAmount));
        }
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

    private void Jump()
    {
        if(rb.gravityScale > 0)
        {
            float speedBoost = Mathf.Abs(rb.velocity.x) / 2f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce + speedBoost);  
        }
        else if(rb.gravityScale < 0)
        {
            float speedBoost = Mathf.Abs(rb.velocity.x) / 2f;
            rb.velocity = new Vector2(rb.velocity.x, -jumpForce - speedBoost);  
        }
        ModifyStamina(-jumpStaminaCost);
    }

    private void TryDash()
    {
        if (!isDashing && CurrentStamina >= dashStaminaCost && !isInvincible)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;
        
        ModifyStamina(-dashStaminaCost);
        
        float dashDirection = horizontalInput != 0 ? Mathf.Sign(horizontalInput) : transform.localScale.x > 0 ? 1 : -1;
        
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(dashDirection * dashForce, 0f), ForceMode2D.Impulse);
        
        if(invincibilityCoroutine != null) StopCoroutine(invincibilityCoroutine);
        invincibilityCoroutine = StartCoroutine(InvincibilityEffect());
        
        yield return new WaitForSeconds(dashDuration);
        
        isDashing = false;
        
        yield return new WaitForSeconds(invincibilityDuration - dashDuration);
        isInvincible = false;
    }

    private IEnumerator InvincibilityEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float blinkSpeed = 0.1f;
        
        while(isInvincible)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(blinkSpeed);
            spriteRenderer.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(blinkSpeed);
        }
        
        spriteRenderer.color = Color.white;
    }

    public bool IsInvincible()
    {
        return isInvincible;
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