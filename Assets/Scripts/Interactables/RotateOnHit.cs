using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Enfabler.Attacking;

public class RotateOnHit : MonoBehaviour, IDamageable
{
    public HitReactData hitReactData;
    public float hitRotateInterval = 45;
    CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        HitReactionDelegate += ObjectHit;
    }

    public MonoBehaviour GetScript()
    {
        return this;
    }

    public E_DamageEvents Damage(ICanDealDamage attacker, int damage, Vector3 spawnPos, Vector3 spawnRot, E_AttackType attackType = E_AttackType.None)
    {
        HitReactionDelegate(damage, Vector3.zero);

        Instantiate(hitReactData.blockFX, spawnPos, Quaternion.Euler(spawnRot));
        if (hitReactData.hitClip != null)
            PlaySoundEffect(hitReactData.hitClip, hitReactData.hitVolume);
        float impulseStrength = Mathf.Clamp(damage * hitReactData.hitImpulseMultiplier, 0, hitReactData.impulseMax);
        SpawnImpulse(impulseStrength);

        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += hitRotateInterval;
        transform.rotation = Quaternion.Euler(rot);

        return E_DamageEvents.Block;
    }

    public delegate void HitDelegate(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None);
    public HitDelegate HitReactionDelegate;

    void ObjectHit(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None)
    {
        //Empty Delegate Function
    }

    void PlaySoundEffect(AudioClip clip, float volume = 1)
    {
        if (AudioManager.instance == null) return;

        AudioManager.instance.PlaySoundEffect(clip, volume);
    }

    public void SpawnImpulse(float impulseStrength)
    {
        if (impulseSource == null) return;

        impulseSource.GenerateImpulseWithForce(impulseStrength);
    }

    #region Unused Interface Methods

    public bool CheckKill()
    {
        return false;
    }

    public bool IsDead()
    {
        return false;
    }

    public void Kill(Vector3 attacker, int damage)
    {
        return;
    }

    #endregion
}