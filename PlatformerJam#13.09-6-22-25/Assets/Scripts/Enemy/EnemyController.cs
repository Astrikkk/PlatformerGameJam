using System.Collections;
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

    [Header("Combat Settings")]
    [SerializeField] private float shootingInterval = 2f;
    [SerializeField] private float shootingRange = 4f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 7f;
    [SerializeField] private int projectileDamage = 1;

    void FixedUpdate()
    {
        if (isDead) return;

        if (isChasing) detectionRange = 20f;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (isWarrior)
            {
                isChasing = true;
                ChasePlayer();
                TryShoot();
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

    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void TryShoot()
    {
        if (Time.time >= nextShootTime)
        {
            Shoot();
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
    }

    IEnumerator Hide()
    {
        
        while (Vector2.Distance(transform.position, moveSpots[spot].position) > 0.7f)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveSpots[spot].position, speed*5f * Time.deltaTime);
            yield return null;
        }

        
        Debug.Log("Playing hide animation");
        
    }

    public void StartDisappearing()
    {
        StartCoroutine(Dissappear());
    }

    public IEnumerator Dissappear()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}