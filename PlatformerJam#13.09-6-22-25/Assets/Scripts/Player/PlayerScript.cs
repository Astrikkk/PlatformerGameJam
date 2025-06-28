using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private float hpRegenRate = 10f;
    [SerializeField] private GameObject hpBar;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private int flashCount = 3;
    public int CurrentHP;
    [SerializeField] private bool canEat;
    [SerializeField] private GameObject deathPanel;
    EnemyController enemyNear;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;
    private Color originalColor;
    private bool isFlashing = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        CurrentHP = maxHP;
        originalColor = spriteRenderer.color;
    }

    void FixedUpdate()
    {
        hpBar.GetComponent<Scrollbar>().size = CurrentHP * 0.01f;
        if (CurrentHP < maxHP && canEat && Input.GetKey(KeyCode.E))
        {
            Eating();
            animator.SetTrigger("Attack");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemyNear = collision.GetComponent<EnemyController>();
        if (enemyNear != null && enemyNear.isEdible && enemyNear.isDead)
        {
            canEat = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyController>() == enemyNear)
        {
            canEat = false;
        }
    }

    void Eating()
    {
        if (enemyNear == null) return;

        enemyNear.StartDisappearing();
        float regenAmount = hpRegenRate * Time.fixedDeltaTime;
        ModifyHP(Mathf.RoundToInt(regenAmount));
    }

    public void ModifyHP(int amount)
    {
        CurrentHP = Mathf.Clamp(CurrentHP + amount, 0, maxHP);
    }

    public void TakeDamage(int amount, Vector2 damageSourcePosition)
    {
        if (GetComponent<PlayerMovement>().IsInvincible()) return;

        ModifyHP(-amount);
        ApplyKnockback(damageSourcePosition);

        if (!isFlashing) StartCoroutine(FlashRed());
        if (CurrentHP <= 0) Death();
    }

    private void ApplyKnockback(Vector2 damageSourcePosition)
    {
        Vector2 knockbackDirection = (Vector2)transform.position - damageSourcePosition;
        knockbackDirection.Normalize();
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
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
        animator.SetTrigger("Die");
        deathPanel.SetActive(true);
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
        this.enabled = false;
    }
}