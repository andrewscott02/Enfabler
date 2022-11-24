using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    public HealthSlider healthSlider;
    CharacterCombat combat;
    public Object bloodFX, parryFX;
    public int maxHealth = 50;
    int currentHealth = 0;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
            healthSlider.ChangeSliderValue(currentHealth, maxHealth);
        combat = GetComponent<CharacterCombat>();
    }

    public void Damage(int damage, Vector3 spawnPos, Vector3 spawnRot)
    {
        if (combat != null)
        {
            if (combat.GetParrying())
            {
                Instantiate(parryFX, spawnPos, Quaternion.Euler(spawnRot));
                return;
            }
        }

        if (bloodFX != null)
        {
            Instantiate(bloodFX, spawnPos, Quaternion.Euler(spawnRot));
        }

        currentHealth -= damage;

        if (healthSlider != null)
            healthSlider.ChangeSliderValue(currentHealth, maxHealth);

        if (CheckKill())
            Kill();
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        if (healthSlider != null)
            healthSlider.ChangeSliderValue(currentHealth, maxHealth);
    }

    public bool CheckKill()
    {
        return currentHealth <= 0;
    }

    public void Kill()
    {
        AIManager.instance.CharacterDied(this.GetComponent<CharacterController>());
    }
}