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

    public SliderScript healthSlider;
    CharacterCombat combat;
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

    public MonoBehaviour GetScript()
    {
        return this;
    }

    [ContextMenu("Take10Damage")]
    public void Take10Damage()
    {
        Damage(GetComponent<CharacterCombat>(), 10, transform.position, transform.eulerAngles);
    }

    public E_DamageEvents Damage(ICanDealDamage attacker, int damage, Vector3 spawnPos, Vector3 spawnRot)
    {
        MonoBehaviour attackerMono = attacker.GetScript();

        Debug.Log(gameObject.name + " was hit");

        if (combat != null)
        {
            if (combat.GetDodging() && attacker.HitDodged()) return E_DamageEvents.Dodge;

            if (combat.parrying && attacker.HitParried())
            {
                if (hitReactData.parryFX != null) { Instantiate(hitReactData.parryFX, spawnPos, Quaternion.Euler(spawnRot)); }
                ParryReaction();
                combat.ParrySuccess();
                return E_DamageEvents.Parry;
            }
            else if (combat.CanBlock() && attacker.HitBlocked())
            {
                if (hitReactData.blockFX != null) { Instantiate(hitReactData.blockFX, spawnPos, Quaternion.Euler(spawnRot)); }
                HitReaction(damage);
                combat.ConsumeArmour();
                return E_DamageEvents.Block;
            }
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
            Vector3 forceOrigin = attacker != null ? attackerMono.gameObject.transform.position : spawnPos;
            Kill(forceOrigin, damage);
            if (hitReactData.deathFX != null)
            {
                if (hitReactData.deathFXGO != null)
                    Instantiate(hitReactData.deathFX, hitReactData.deathFXGO.transform);
                else
                    Instantiate(hitReactData.deathFX, spawnPos, Quaternion.Euler(spawnRot));
            }
        }
        else
        {
            HitReaction(damage);
            if (hitReactData.bloodFX != null)
            {
                Instantiate(hitReactData.bloodFX, spawnPos, Quaternion.Euler(spawnRot));
            }
        }

        return E_DamageEvents.Hit;
    }

    void HitReaction(int damage)
    {
        if (combat == null) { return; }

        combat.GotHit();

        if (animator != null && damage >= hitReactData.lightHitReactThreshold)
        {
            combat.ForceEndAttack();
            animator.SetBool("InHitReaction", true);
            animator.SetTrigger(damage < hitReactData.heavyHitReactThreshold ? "HitReactLight" : "HitReactHeavy");
            if (damage > hitReactData.heavyHitReactThreshold)
            {
                if (hitReactData.hitClip != null)
                    PlaySoundEffect(hitReactData.hitClip, hitReactData.hitVolume);
            }
        }
        else
        {
            combat.ResetAttack();
        }

        float impulseStrength = Mathf.Clamp(damage * hitReactData.hitImpulseMultiplier, 0, hitReactData.impulseMax);
        SpawnImpulse(impulseStrength);
        
        Slomo(hitReactData.hitSlomoScale, hitReactData.hitSlomoDuration);
    }

    void PlaySoundEffect(AudioClip clip, float volume = 1)
    {
        if (AudioManager.instance == null) return;

        AudioManager.instance.PlaySoundEffect(clip, volume);
    }

    void ParryReaction()
    {
        SpawnImpulse(hitReactData.parryImpulseStrength);
        Slomo(hitReactData.parrySlomoScale, hitReactData.parrySlomoDuration);
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
        if (hitReactData.hitClip != null)
            PlaySoundEffect(hitReactData.hitClip, hitReactData.hitVolume);

        if (combat != null)
        {
            combat.ForceEndAttack();
            if (combat.armourSlider != null)
                combat.armourSlider.gameObject.SetActive(false);
            combat.enabled = false;
        }

        if (controller != null)
        {
            controller.Killed();

            if (hitReactData.killRagdoll)
            {
                //Debug.Log(gameObject + "has controller");
                ExplosiveForceData forcedata = new ExplosiveForceData()
                {
                    explosiveForce = damage,
                    origin = attacker
                };

                controller.ActivateRagdoll(true, forcedata, !hitReactData.killAnim);
                gameObject.name += " -- Dead";
            }

            controller.enabled = false;
        }

        if (hitReactData.killAnim)
        {
            controller.rb.constraints = RigidbodyConstraints.FreezeAll;
            controller.enabled = false;
            animator.SetTrigger("Death");
        }

        if (hitReactData.killDestroyTime >= 0)
        {
            //Debug.Log(gameObject + "has no controller");
            Destroy(this.gameObject, hitReactData.killDestroyTime);
        }

        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);
    }

    public bool IsDead() { return dying; }

    CinemachineImpulseSource impulseSource;

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