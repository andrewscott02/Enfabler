using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public ConstructPlayerModel modelConstructor;

    public HealthSlider healthSlider;
    CharacterCombat combat;
    public Object bloodFX, parryFX;
    public int maxHealth = 50;
    int currentHealth = 0; public int GetCurrentHealth() { return currentHealth; }

    AIController AIController;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
            healthSlider.ChangeSliderValue(currentHealth, maxHealth);
        combat = GetComponent<CharacterCombat>();

        AIController = GetComponent<AIController>();
    }

    public void Damage(CharacterCombat attacker, int damage, Vector3 spawnPos, Vector3 spawnRot)
    {
        if (combat != null)
        {
            if (combat.GetDodging())
            {
                return;
            }

            if (combat.GetParrying())
            {
                if (parryFX != null) { Instantiate(parryFX, spawnPos, Quaternion.Euler(spawnRot)); }
                if (attacker != null) { attacker.Parried(); }
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

        if (modelConstructor != null)
            modelConstructor.PlayerHit();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        if (CheckKill())
        {
            Kill();
        }
        else
        {
            HitReaction();
        }
    }

    void HitReaction()
    {
        if (combat != null) { combat.canAttack = false; }
        else { Debug.LogWarning("No combat script"); }

        if (animator != null)
        {
            animator.SetTrigger("HitReact");
            animator.SetInteger("RandReact", Random.Range(0, animator.GetInteger("RandReactMax") + 1));
        }
        else { Debug.LogWarning("No animator"); }
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