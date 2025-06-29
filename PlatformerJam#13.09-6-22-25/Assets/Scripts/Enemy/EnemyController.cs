using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool isEdible;
    public bool isDead;
    [SerializeField] private bool isWarrior;
    [SerializeField] private float speed;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private int spot = 0;
    [SerializeField] private Transform[] moveSpots;
    [SerializeField] private Transform player;

    private bool isHiding = false;
    private bool isChasing = false;
    private float nextShootTime;
    public int HpCount = 100;

    private SpriteRenderer spriteRenderer;
    private Animator animator;


    private Rigidbody2D rb;

    [Header("Combat Settings")]
    [SerializeField] private float shootingInterval = 2f;
    [SerializeField] private float shootingRange = 4f;
    [SerializeField] private float stoppingDistance = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 7f;
    [SerializeField] private int projectileDamage = 1;

    private Color originalColor;
    private bool isFlashing = false;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private Sprite deadSprite;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (HpCount <= 0) Death();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isLookingRight = player.position.x >= transform.position.x;
        spriteRenderer.flipX = !isLookingRight;

        if (distanceToPlayer <= detectionRange)
        {
            if (isWarrior)
            {
                isChasing = true;
                animator.SetBool("isMoving", true);

                if (distanceToPlayer > stoppingDistance + 0.2f)
                {
                    ChasePlayer();
                }

                if (distanceToPlayer <= shootingRange)
                {
                    TryShoot();
                }
            }
            else if (!isHiding)
            {
                isHiding = true;
                spot = 1;
                StartCoroutine(Hide());
            }
        }
        else
        {
            isChasing = false;
            animator.SetBool("isMoving", false);

            if (!isHiding)
            {
                Patrol();
            }
        }

        if (!isHiding && !isChasing)
        {
            Patrol();
        }
    }


    void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, moveSpots[spot].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, moveSpots[spot].position) <= 1f)
        {
            if (spot == 1)
                spot = 0;
            else if (spot == 0)
                spot = 1;
        }
    }

    public void InvertGravity()
    {
        spriteRenderer.flipY = !spriteRenderer.flipY;
    }

    public void ModifyHP(int amount)
    {
        HpCount = Mathf.Clamp(HpCount + amount, 0, 100);
    }

    public void TakeDamage(int amount, Vector2 damageSourcePosition)
    {

        ModifyHP(-amount);
        ApplyKnockback(damageSourcePosition);
        if (!isFlashing) StartCoroutine(FlashRed());
    }

    private void ApplyKnockback(Vector2 damageSourcePosition)
    {
        Vector2 knockbackDirection = (Vector2)transform.position - damageSourcePosition;
        knockbackDirection.Normalize();
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
    }

    void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    void TryShoot()
    {
        if (Time.time >= nextShootTime && Vector3.Distance(transform.position, player.position) <= shootingRange)
        {
            Shoot();
            animator.SetTrigger("Attacking");
            nextShootTime = Time.time + shootingInterval;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Projectile prefab or fire point not assigned!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Vector2 direction = (player.position - firePoint.position).normalized;

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
            projectileScript.SetSpeed(projectileSpeed);
            projectileScript.SetDamage(projectileDamage);
        }
        else
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
        }
    }

    IEnumerator Hide()
    {
        while (Vector2.Distance(transform.position, moveSpots[spot].position) > 0.7f)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveSpots[spot].position, speed * 5f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FlashRed()
    {
        isFlashing = true;
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
        isFlashing = false;
    }

    void Death()
    {
        animator.SetBool("isDead", false);
        animator.SetBool("isMoving", false);
        animator.ResetTrigger("Attacking");
        animator.SetTrigger("Die");

        spriteRenderer.sprite = deadSprite;
        Destroy(animator);
        StopAllCoroutines();

        isDead = true;
    }

    public void StartDisappearing()
    {
        StartCoroutine(Dissappear());
    }

    public IEnumerator Dissappear()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}