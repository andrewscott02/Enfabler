using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHit : MonoBehaviour
{
    public GameObject projectileAttach;
    bool alreadyAttached = false;

    public GameObject caster;
    public ICanDealDamage casterDamage;
    public TrapStats trapStats;
    public int damage;
    public ProjectileMovement move;

    public LayerMask layerMask;
    public LayerMask attachMask;
    List<IDamageable> hitTargets = new List<IDamageable>();

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            //Debug.Log("Projectile hit " + other.gameObject.name);
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
                //Debug.LogWarning("No interface");
                Attach(other);
                Destroy(move.gameObject);
                return;
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
            Attach(other);
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

        if (trapStats.shotAOE == 0)
        {
            //only hit target
            hitData = casterDamage.DealDamage(target, damage, transform.position, transform.rotation.eulerAngles);
        }
        else
        {
            //TODO: Affect AOE targets
            //Spawn impulse
        }

        bool parrySuccess = false;
        if (hitData == E_DamageEvents.Parry)
        {
            if (targetMono.gameObject != null)
            {
                move.Fire(caster.gameObject.transform.position, trapStats, targetMono.gameObject);
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
        {
            if (trapStats.explosionFX != null)
                Instantiate(trapStats.explosionFX, transform.position, transform.rotation);
            Destroy(move.gameObject);
        }
    }

    void Attach(Collider other)
    {
        //Debug.Log("Attach to object");
        if (alreadyAttached || projectileAttach == null) return;

        Vector3 pos = projectileAttach.transform.position;
        Quaternion rot = projectileAttach.transform.rotation;

        RaycastHit hit;

        Vector3 origin = HelperFunctions.GetFlankingPoint(projectileAttach.transform.position, move.lastPos, move.projectileSpeed * Time.fixedDeltaTime);
        float distance = move.projectileSpeed * Time.fixedDeltaTime;
        Vector3 dir = (projectileAttach.transform.position - move.lastPos).normalized;
        if (Physics.SphereCast(origin, 0.45f, direction: dir, out hit, maxDistance: distance, attachMask))
        {
            Debug.Log("Attach Hit: " + hit.collider.gameObject);
            projectileAttach.transform.parent = hit.collider.gameObject.transform;

            projectileAttach.transform.position = hit.point;
            //projectileAttach.transform.rotation = Quaternion.Euler(dir);
        }
        else
        {
            Debug.Log("Attach did not hit: " + other.gameObject);

            projectileAttach.transform.parent = other.gameObject.transform;

            projectileAttach.transform.position = pos;
            projectileAttach.transform.rotation = rot;
        }

        alreadyAttached = true;
    }
}
