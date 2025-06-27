using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int meleeDamage = 20;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackStaminaCost = 15;
    [SerializeField] private int dashAttackStaminaCost = 25;
    [SerializeField] private int jumpAttackStaminaCost = 20;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Vector2 attackOffset = new Vector2(0.5f, 0f);

    private PlayerMovement playerMovement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            if (playerMovement.IsDashing() && playerMovement.CurrentStamina >= dashAttackStaminaCost)
            {
                PerformDashAttack();
            }
            else if (!playerMovement.IsGrounded() && playerMovement.CurrentStamina >= jumpAttackStaminaCost)
            {
                PerformJumpAttack();
            }
            else if (playerMovement.CurrentStamina >= attackStaminaCost)
            {
                PerformRegularAttack();
            }
        }
    }

    private void PerformRegularAttack()
    {
        lastAttackTime = Time.time;
        playerMovement.ModifyStamina(-attackStaminaCost);
        animator.SetTrigger("Attack");
        StartCoroutine(AttackRoutine(1f));
    }

    private void PerformDashAttack()
    {
        lastAttackTime = Time.time;
        playerMovement.ModifyStamina(-dashAttackStaminaCost);
        animator.SetTrigger("DashAttack");
        StartCoroutine(AttackRoutine(1.5f));
    }

    private void PerformJumpAttack()
    {
        lastAttackTime = Time.time;
        playerMovement.ModifyStamina(-jumpAttackStaminaCost);
        animator.SetTrigger("JumpAttack");
        StartCoroutine(AttackRoutine(1.3f));
    }

    private IEnumerator AttackRoutine(float damageMultiplier)
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.2f); // Задержка перед ударом

        Vector2 attackPos = (Vector2)transform.position +
                          new Vector2(attackOffset.x * (spriteRenderer.flipX ? -1 : 1), attackOffset.y);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                int damage = Mathf.RoundToInt(meleeDamage * damageMultiplier);
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                enemyController.TakeDamage(damage, direction);
            }
        }

        yield return new WaitForSeconds(0.3f); // Время завершения анимации
        isAttacking = false;
    }

    public bool IsAttacking() => isAttacking;

    private void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null) return;

        Vector2 attackPos = (Vector2)transform.position +
                          new Vector2(attackOffset.x * (spriteRenderer.flipX ? -1 : 1), attackOffset.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }
}