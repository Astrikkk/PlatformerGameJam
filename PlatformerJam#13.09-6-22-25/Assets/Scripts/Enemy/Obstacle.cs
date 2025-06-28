using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private int damageCost;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerScript Hero = collision.GetComponent<PlayerScript>();
        if(Hero != null )Hero.TakeDamage(damageCost, transform.position);
    }
}
