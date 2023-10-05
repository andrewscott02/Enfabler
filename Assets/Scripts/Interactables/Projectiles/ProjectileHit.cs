using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHit : MonoBehaviour
{
    public GameObject caster;
    public Trap trap;
    public ProjectileMovement move;

    public LayerMask layerMask;
    List<IDamageable> hitTargets = new List<IDamageable>();

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            Debug.Log("Projectile hit " + other.gameObject.name);
            IDamageable hitDamageable = other.GetComponent<IDamageable>();

            //Gets hit damageable from parent if it cannot get it from the game object
            if (hitDamageable == null)
            {
                hitDamageable = other.GetComponentInParent<IDamageable>();
            }

            #region Guard Clauses

            //Return if collided object has no health component
            if (hitDamageable == null)
            {
                Debug.LogWarning("No interface");
                Destroy(move.gameObject);
            }

            //Return if hitting caster
            MonoBehaviour targetMono = hitDamageable.GetScript();
            if (caster == null) { Debug.LogWarning("no caster"); }
            if (targetMono == null) { Debug.LogWarning("no target mono"); }

            if (caster != null && targetMono != null)
            {
                if (caster == targetMono.gameObject)
                {
                    return;
                }
            }

            #endregion

            //If it can be hit, deal damage to target and add it to the hit targets list
            DetermineEffect(hitDamageable);
        }
    }

    bool alreadyHit = false;

    void DetermineEffect(IDamageable target)
    {
        if (alreadyHit) return;
        alreadyHit = true;

        //Debug.Log("hitting target for " + trap.trapStats.damage);
        E_DamageEvents hitData = E_DamageEvents.Hit;
        MonoBehaviour targetMono = target.GetScript();

        if (trap.trapStats.shotAOE == 0)
        {
            //only hit target
            hitData = trap.ApplyEffect(target, targetMono.gameObject.transform.position);
        }
        else
        {
            //TODO: Affect AOE targets
            //Spawn impulse
        }

        bool parrySuccess = false;
        if (hitData == E_DamageEvents.Parry)
        {
            //TODO: Reflect projectile
            if (targetMono.gameObject != null)
            {
                move.Fire(trap.gameObject.transform.position, trap, targetMono.gameObject);
                caster = targetMono.gameObject;
                parrySuccess = true;
                alreadyHit = false;
            }
            else
            {
                Debug.LogWarning("NoCaster");
            }
        }

        if (!parrySuccess)
            Destroy(move.gameObject);
    }
}
