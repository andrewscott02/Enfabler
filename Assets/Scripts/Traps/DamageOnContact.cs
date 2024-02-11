using Enfabler.Attacking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnContact : MonoBehaviour, ICanDealDamage
{
    public TrapStats trapStats;
    public LayerMask layerMask;

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            IDamageable hitDamageable = other.GetComponent<IDamageable>();

            if (hitDamageable == null)
            {
                hitDamageable = other.GetComponentInParent<IDamageable>();
            }

            #region Guard Clauses

            //Return if collided object has no health component
            if (hitDamageable == null)
            {
                Debug.LogWarning("No interface");
                return;
            }

            #endregion

            //If it can be hit, deal damage to target and add it to the hit targets list
            DealDamage(hitDamageable, trapStats.damage, transform.position, new Vector3(0, 0, 0));
        }
    }

    public MonoBehaviour GetScript()
    {
        return this;
    }

    public E_DamageEvents DealDamage(IDamageable target, int damage, Vector3 spawnPos, Vector3 spawnRot, E_AttackType attackType = E_AttackType.None)
    {
        return target.Damage(this, damage, spawnPos, spawnRot);
    }

    public bool HitDodged()
    {
        return false;
    }

    public bool HitBlocked(IDamageable other)
    {
        return trapStats.canBlock;
    }

    public bool HitParried(IDamageable other)
    {
        return trapStats.canParry;
    }
}
