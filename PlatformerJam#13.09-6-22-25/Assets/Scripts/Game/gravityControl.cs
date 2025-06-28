using System.Collections;
using UnityEngine;

public class gravityControl : MonoBehaviour
{
    [SerializeField] private camController _cm;
    [SerializeField] private float cooldownTime = 1.0f;
    [SerializeField] private bool enableRandomInversion = false;
    [SerializeField] private float minRandomDelay = 10f;
    [SerializeField] private float maxRandomDelay = 60f;

    private bool isOnCooldown = false;
    private PlayerMovement player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>();

        if (enableRandomInversion)
        {
            StartCoroutine(RandomInvertCoroutine());
        }
    }

    public void InvertGravityForAllRigidbodies()
    {
        if (isOnCooldown) return;

        Rigidbody2D[] allRigidbodies = FindObjectsOfType<Rigidbody2D>();

        foreach (Rigidbody2D rb in allRigidbodies)
        {
            rb.gravityScale = -rb.gravityScale;
        }

        _cm.changeGravity();
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    void Invert()
    {
        InvertGravityForAllRigidbodies();
        player.InvertGravity();
    }

    private IEnumerator RandomInvertCoroutine()
    {
        while (true)
        {
            float randomDelay = Random.Range(minRandomDelay, maxRandomDelay);
            yield return new WaitForSeconds(randomDelay);

            if (!isOnCooldown)
            {
                Invert();
            }
        }
    }
}