using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapApplyEffect : MonoBehaviour
{
    Trap trap;
    PlayerController player;

    private void Awake()
    {
        trap = GetComponentInParent<Trap>();
    }

    public void Activate()
    {
        DetermineEffect();
    }

    public LayerMask layerMask;
    List<IDamageable> affectTargets = new List<IDamageable>();
    List<IDamageable> hitTargets = new List<IDamageable>();

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            if (!trap.trapStats.onlyHitPlayer || other.gameObject.CompareTag("Player"))
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
                if (!affectTargets.Contains(hitDamageable))
                    affectTargets.Add(hitDamageable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            if (!trap.trapStats.onlyHitPlayer || other.gameObject.CompareTag("Player"))
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
                affectTargets.Remove(hitDamageable);
            }
        }
    }

    void DetermineEffect()
    {
        switch (trap.trapStats.targetType)
        {
            case E_TargetType.Shot:
                //Affect only 1 player
                foreach (var item in affectTargets)
                {
                    MonoBehaviour targetMono = item.GetScript();
                    if (Vector3.Distance(transform.position, targetMono.transform.position) <= trap.trapStats.range)
                    {
                        Debug.Log("Spawning projectile at " + item.GetScript().gameObject);
                        CapsuleCollider targetCol = targetMono.GetComponent<CapsuleCollider>();
                        SpawnProjectile(targetCol.bounds.center);
                        //CheckSight(targetMono.gameObject);
                        return;
                    }
                }
                break;
            case E_TargetType.Area:
                //Affect all
                if (trap.trapStats.explosionFX != null)
                    Instantiate(trap.trapStats.explosionFX, transform.position, transform.rotation);
                foreach (var item in affectTargets)
                {
                    if (!hitTargets.Contains(item))
                    {
                        MonoBehaviour targetMono = item.GetScript();
                        trap.ApplyEffect(item, targetMono.gameObject.transform.position);

                        hitTargets.Add(item);
                    }
                }
                break;
            default:
                break;
        }

        hitTargets.Clear();
    }

    public LayerMask sightLayerMask;

    void CheckSight(GameObject target)
    {
        //Raycast between sword base and tip
        RaycastHit hit;

        Vector3 origin = transform.position;
        CapsuleCollider targetCol = target.GetComponent<CapsuleCollider>();
        float distance = Vector3.Distance(transform.position, targetCol.bounds.center);
        Vector3 dir = targetCol.bounds.center - transform.position;

        if (Physics.SphereCast(origin, radius: 0.4f, direction: dir, out hit, maxDistance: distance, sightLayerMask))
        {
            if (hit.collider.gameObject == target.gameObject)
                SpawnProjectile(targetCol.bounds.center);
        }
        else
        {
            SpawnProjectile(targetCol.bounds.center);
        }
    }

    void SpawnProjectile(Vector3 targetPos)
    {
        Vector3 spawnPos = transform.position;

        GameObject projectileObj = Instantiate(trap.trapStats.projectile, spawnPos, transform.rotation) as GameObject;
        ProjectileMovement projectileMove = projectileObj.GetComponent<ProjectileMovement>();
        projectileMove.Fire(targetPos, trap.trapStats, trap.gameObject);
    }
}
