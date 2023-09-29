using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour, ICanDealDamage
{
    public TrapStats trapStats;
    TrapApplyEffect applyEffect;
    SphereCollider applyCollider;

    private void Awake()
    {
        applyEffect = GetComponentInChildren<TrapApplyEffect>();
        applyCollider = applyEffect.GetComponent<SphereCollider>();
        applyCollider.radius = trapStats.range;

        applyEffect.Deactivate();
    }

    public void ActivateTrap()
    {
        switch (trapStats.durationType)
        {
            case E_Duration.OnlyOnce:
                EffectTrigger();
                break;
            case E_Duration.OnceDestroy:
                EffectTrigger();
                DeactivateTrap();
                Destroy(this.gameObject, 0.15f);
                break;
            case E_Duration.Interval:
                TrapManager.instance.TrapActivation += EffectTrigger;
                break;
            default:

                break;
        }
    }

    public void DeactivateTrap()
    {
        if (trapStats.durationType == E_Duration.Interval)
        {
            TrapManager.instance.TrapActivation -= EffectTrigger;
        }
    }

    void EffectTrigger()
    {
        if (trapStats.durationType == E_Duration.Interval)
        {
            if (TrapManager.instance.trapTime % trapStats.activateInterval != 0)
                return;
        }

        applyEffect.Activate();
    }

    public void ApplyEffect(IDamageable damageable, Vector3 hitPos)
    {
        DealDamage(damageable, trapStats.damage, hitPos, new Vector3(0, 0, 0));
    }

    private void OnDrawGizmosSelected()
    {
        if (trapStats != null)
            Gizmos.DrawWireSphere(transform.position, trapStats.range);
    }

    public void DealDamage(IDamageable target, int damage, Vector3 spawnPos, Vector3 spawnRot)
    {
        target.Damage(this, damage, spawnPos, spawnRot);
    }

    public bool HitDodged()
    {
        return false;
    }

    public bool HitParried()
    {
        return false;
    }
}
