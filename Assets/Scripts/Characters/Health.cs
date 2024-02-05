using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Enfabler.Attacking;

public class Health : MonoBehaviour, IDamageable, IHealable
{
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public ConstructPlayerModel modelConstructor;

    public SliderScript healthSlider;
    public Armour armourScript;
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

        HitReactionDelegate += HitReaction;
        killDelegate += Kill;

        armourScript = GetComponent<Armour>();
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

    public E_DamageEvents Damage(ICanDealDamage attacker, int damage, Vector3 spawnPos, Vector3 spawnRot, E_AttackType attackType = E_AttackType.None)
    {
        MonoBehaviour attackerMono = attacker.GetScript();
        Vector3 dir = transform.position - attackerMono.transform.position;
        dir.Normalize();

        //Debug.Log(gameObject.name + " was hit");

        if (combat != null)
        {
            if (combat.GetDodging() && attacker.HitDodged()) return E_DamageEvents.Dodge;

            if (combat.parrying && attacker.HitParried(this))
            {
                if (hitReactData.parryFX != null) { Instantiate(hitReactData.parryFX, spawnPos, Quaternion.Euler(spawnRot)); }
                ParryReaction();
                combat.parriedDelegate();
                return E_DamageEvents.Parry;
            }
            else if (combat.CanBlock() && attacker.HitBlocked(this))
            {
                if (hitReactData.blockFX != null) { Instantiate(hitReactData.blockFX, spawnPos, Quaternion.Euler(spawnRot)); }
                HitReactionDelegate(damage, dir);
                combat.blockedDelegate();
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
            killDelegate(forceOrigin, damage);
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
            HitReactionDelegate(damage, dir, attackType);
            if (hitReactData.bloodFX != null)
            {
                Instantiate(hitReactData.bloodFX, spawnPos, Quaternion.Euler(spawnRot));
            }
        }

        return E_DamageEvents.Hit;
    }

    public delegate void HitDelegate(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None);
    public HitDelegate HitReactionDelegate;

    void HitReaction(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None)
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

    public delegate void KillDelegate(Vector3 attacker, int damage);
    public KillDelegate killDelegate;

    public void Kill(Vector3 attacker, int damage)
    {
        dying = true;
        SpawnImpulse(hitReactData.killImpulseStrength);

        //Debug.Log("Enemies left " + AIManager.instance.GetEnemiesInCombat());
        if (AIManager.instance.GetEnemiesInCombat() == 1)
        {
            if (hitReactData.hitClip != null)
                PlaySoundEffect(hitReactData.hitClip, hitReactData.hitVolume * 2);
            KillCamSlowMo();
        }
        else
        {
            if (hitReactData.hitClip != null)
                PlaySoundEffect(hitReactData.hitClip, hitReactData.hitVolume);
            Slomo(hitReactData.killSlomoScale, hitReactData.killSlomoDuration);
        }

        if (combat != null)
        {
            combat.ForceEndAttack();
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

        Debug.Log("Set slow motion to " + slomoStrength.ToString());
        TimeManager.instance.SetTimeScale(slomoStrength, slomoDuration);
    }

    public void KillCamSlowMo()
    {
        Debug.Log("kill cam slomo");
        Slomo(0.2f, 2f);
        CameraManager.instance.SetCombatCam(false, true);
        AudioManager.instance.ExploreMusicFade();
    }
}