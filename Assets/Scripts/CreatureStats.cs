using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStats : MonoBehaviour
{
    public float maxHealth = 5;
    public float health;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
            { Die(); }
    }

    public void Heal(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
            { health = maxHealth; }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died. RIP");
    }
}
