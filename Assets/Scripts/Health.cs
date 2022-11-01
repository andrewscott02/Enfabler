using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    public int maxHealth = 50;
    int currentHealth = 0;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        if (CheckKill())
            Kill();
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
    }

    public bool CheckKill()
    {
        return currentHealth <= 0;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}