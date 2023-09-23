using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

    public HitReactData hitReactData;

    BaseCharacterController controller;
    AIController AIController;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
            healthSlider.ChangeSliderValue(currentHealth, maxHealth);
        combat = GetComponent<CharacterCombat>();

        controller = GetComponent<BaseCharacterController>();
        AIController = GetComponent<AIController>();
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
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
            Kill(attacker.gameObject.transform.position, damage);
        }
        else
        {
            HitReaction(damage);
        }
    }

    void HitReaction(int damage)
    {
        if (combat != null) { combat.canAttack = false; }
        else { Debug.LogWarning("No combat script"); }

        if (animator != null)
        {
            animator.SetTrigger(damage < hitReactData.heavyHitReactThreshold ? "HitReactLight" : "HitReactHeavy");
        }
        else { Debug.LogWarning("No animator"); }

        SpawnImpulse(damage * hitReactData.hitImpulseMultiplier);
        Slomo(hitReactData.hitSlomoScale, hitReactData.hitSlomoDuration);
    }

    public void Heal(int heal)
    {
        currentHealth = Mathf.Clamp(currentHealth + heal, 0, maxHealth);
        if (healthSlider != null)
            healthSlider.ChangeSliderValue(currentHealth, maxHealth);
    }

    public bool CheckKill()
    {
        return currentHealth <= 0;
    }

    public bool dying = false;

    public void Kill(Vector3 attacker, int damage)
    {
        dying = true;
        SpawnImpulse(hitReactData.killImpulseStrength);
        Slomo(hitReactData.killSlomoScale, hitReactData.killSlomoDuration);
        if (AIManager.instance != null)
            AIManager.instance.CharacterDied(this.GetComponent<BaseCharacterController>());
        else
        {
            if (controller != null)
            {
                Debug.Log(gameObject + "has controller");
                ExplosiveForceData forcedata = new ExplosiveForceData()
                {
                    explosiveForce = damage,
                    origin = attacker
                };
                controller.ActivateRagdoll(true, forcedata);
            }
            else
            {
                Debug.Log(gameObject + "has no controller");
                Destroy(this.gameObject);
            }
        }
    }

    public CinemachineImpulseSource impulseSource;

    public void SpawnImpulse(float impulseStrength)
    {
        if (impulseSource == null) return;

        impulseSource.GenerateImpulseWithForce(impulseStrength);
    }

    public void Slomo(float slomoStrength, float slomoDuration)
    {
        if (TimeManager.instance == null) return;

        TimeManager.instance.SetTimeScale(slomoStrength, slomoDuration);
    }
}