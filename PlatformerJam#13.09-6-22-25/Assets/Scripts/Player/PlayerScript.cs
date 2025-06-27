using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private float hpRegenRate = 10f;
    [SerializeField] private GameObject hpBar;
    public int CurrentHP;
    [SerializeField]private bool canEat;
    EnemyController enemyNear;

    private void Start()
    {
        CurrentHP = maxHP;
    }

    void Update()
    {
        hpBar.GetComponent<Scrollbar>().size = CurrentHP * 0.01f;
        if(CurrentHP < maxHP)
        {
            if(Input.GetKey(KeyCode.E))
            {
                if(canEat)
                    Eating();
            }
        }
        
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemyNear = collision.GetComponent<EnemyController>();
        if (enemyNear != null && enemyNear.isEdible && enemyNear.isDead)
        {
            canEat = true;
        }
        else
        {
            canEat = false;
        }
    }

    void Eating()
    {
        enemyNear.StartDisappearing();
        float regenAmount = hpRegenRate * Time.fixedDeltaTime;
        ModifyHP(Mathf.RoundToInt(regenAmount));
    }

    public void ModifyHP(int amount)
    {
        CurrentHP += amount;
    }

    public void takeDamage(int amount)
    {
        if (GetComponent<PlayerMovement>().IsInvincible()) 
        {
            return;
        }
        //anim
        ModifyHP(-amount);
        if(CurrentHP <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        //anim
        print("isDead");
    }
}
