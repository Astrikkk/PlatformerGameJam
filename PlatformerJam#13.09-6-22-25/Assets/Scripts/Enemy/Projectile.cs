using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;

    public void SetDirection(Vector2 dir) => direction = dir;
    public void SetSpeed(float spd) => speed = spd;
    public void SetDamage(int dmg) => damage = dmg;

    private void Start()
    {
        Invoke("DestroyObject", 5);
    }



    void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript playerscript = other.GetComponent<PlayerScript>();
            if (playerscript != null)
            {
                playerscript.TakeDamage(damage, transform.position);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Other"))
        {
            Destroy(gameObject);
        }
    }


    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}