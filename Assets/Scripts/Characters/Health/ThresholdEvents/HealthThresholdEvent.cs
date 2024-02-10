using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

public class HealthThresholdEvent : MonoBehaviour
{
    Health health;
    HitboxModifier hitBox;

    public int threshold = 0;
    int currentDamage = 0;
    public bool doOnce = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        IDamageable damageable = GetComponent<IDamageable>();

        if (damageable == null)
            return;

        MonoBehaviour mono = damageable.GetScript();

        health = mono as Health;

        hitBox = mono as HitboxModifier;

        AddDelegates();
    }

    protected void AddDelegates()
    {
        if (health != null)
        {
            health.HitReactionDelegate += HitReaction;
        }

        if (hitBox != null)
        {
            hitBox.HitReactionDelegate += HitReaction;
        }
    }

    protected void RemoveDelegates()
    {
        if (health != null)
        {
            health.HitReactionDelegate -= HitReaction;
        }

        if (hitBox != null)
        {
            hitBox.HitReactionDelegate -= HitReaction;
        }
    }

    private void OnDestroy()
    {
        RemoveDelegates();
    }

    void HitReaction(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None)
    {
        currentDamage += damage;

        if (currentDamage >= threshold)
            ThresholdReached();
    }

    protected virtual void ThresholdReached()
    {
        currentDamage = 0;

        if (doOnce)
            RemoveDelegates();
    }
}