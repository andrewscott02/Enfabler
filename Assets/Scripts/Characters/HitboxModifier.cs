using Enfabler.Attacking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxModifier : MonoBehaviour, IDamageable
{
    public bool detachFromParent = false;

    //public int hitBoxHealth;
    public float damageModifier = 1;

    //public bool overrideImmunity = false;
    //public bool immune = false;

    public Health health;

    private void Start()
    {
        if (detachFromParent)
            this.gameObject.transform.SetParent(null, true);

        HitReactionDelegate += HitReaction;

        health.killDelegate += Kill;
    }

    public bool CheckKill()
    {
        //Empty
        return false;
    }

    public delegate void HitDelegate(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None);
    public HitDelegate HitReactionDelegate;

    void HitReaction(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None)
    {
        //Empty Delegate
    }

    public E_DamageEvents Damage(ICanDealDamage attacker, int damage, Vector3 spawnPos, Vector3 spawnRot, E_AttackType attackType = E_AttackType.None)
    {
        if (attacker.GetScript().gameObject != health.gameObject)
        {
            HitReactionDelegate((int)((float)damage * damageModifier), Vector3.zero, attackType);
            return health.Damage(attacker, (int)((float)damage * damageModifier), spawnPos, spawnRot, attackType);
        }

        return E_DamageEvents.Dodge;
    }

    public MonoBehaviour GetScript()
    {
        return this;
    }

    public bool IsDead()
    {
        return false;
    }

    public void Kill(Vector3 attacker, int damage)
    {
        Destroy(gameObject);
    }
}
