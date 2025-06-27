using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool isEdible;
    public bool isDead;
    [SerializeField] private bool isWarrior;
    [SerializeField] private float speed;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LayerMask obstacleLayers;

    private Vector3 dir;
    private Transform player;
    private bool playerDetected;
    private float lastShootTime;

    void Start()
    {
        dir = transform.right;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        CheckForPlayer();
        
        if (playerDetected)
        {
            if (isWarrior)
            {
                WarriorBehavior();
            }
            else
            {
                FleeBehavior();
            }
        }
        else
        {
            Move();
        }
    }

    void CheckForPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange)
        {
            playerDetected = false;
            return;
        }

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer,
            detectionRange,
            obstacleLayers
        );


        if (hit.collider == null || hit.collider.CompareTag("Player"))
        {
            playerDetected = true;
            Debug.DrawRay(transform.position, directionToPlayer * detectionRange, Color.green);
        }
        else
        {
            playerDetected = false;
            Debug.DrawRay(transform.position, directionToPlayer * detectionRange, Color.red);
        }
    }

    void WarriorBehavior()
    {
        // Поворачиваемся к игроку
        dir = (player.position - transform.position).normalized;
        
        // Стреляем с задержкой
        if (Time.time - lastShootTime > shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void FleeBehavior()
    {
        dir = (transform.position - player.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
    }

    void Shoot()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(Vector3.forward, dir));
        }
    }

    void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, 0.1f);

        if (colliders.Length > 0) dir *= -1f;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
    }

    public void StartDisappearing()
    {
        StartCoroutine(Disappear());
    }

    public IEnumerator Disappear()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}