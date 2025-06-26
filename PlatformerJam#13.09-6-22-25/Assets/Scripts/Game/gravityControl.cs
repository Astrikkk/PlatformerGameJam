using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityControl : MonoBehaviour
{
    [SerializeField] private camController _cm;
    [SerializeField] private float cooldownTime = 1.0f; 
    private bool isOnCooldown = false;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isOnCooldown)
        {
            InvertGravityForAllRigidbodies();
        }
    }
}