using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapApplyEffect : MonoBehaviour
{
    Trap trap;
    Collider effectCollider;
    PlayerController player;

    private void Awake()
    {
        trap = GetComponentInParent<Trap>();
        effectCollider = GetComponent<Collider>();
    }

    public void Activate()
    {
        affectTargets = new List<IDamageable>();
        hitTargets = new List<IDamageable>();
        effectCollider.enabled = true;
        StartCoroutine(ITriggerInstantEffect(0.1f));
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        affectTargets = new List<IDamageable>();
        hitTargets = new List<IDamageable>();
        effectCollider.enabled = false;
    }

    IEnumerator ITriggerInstantEffect(float delay)
    {
        yield return new WaitForSeconds(delay);
        DetermineEffect();
        Deactivate();
    }

    public LayerMask layerMask;
    List<IDamageable> affectTargets;
    List<IDamageable> hitTargets;

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
            affectTargets.Add(hitDamageable);
        }
    }

    private void OnTriggerExit(Collider other)
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
            affectTargets.Remove(hitDamageable);
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
                    PlayerController player = targetMono.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        if (Vector3.Distance(transform.position, player.transform.position) <= trap.trapStats.range)
                        {
                            CapsuleCollider playerCol = player.GetComponent<CapsuleCollider>();
                            Vector3 targetPos = playerCol.bounds.center;
                            SpawnProjectile(targetPos);
                            return;
                        }
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
    }

    void SpawnProjectile(Vector3 targetPos)
    {
        Vector3 spawnPos = HelperFunctions.GetFlankingPoint(targetPos, transform.position, 1f);

        GameObject projectileObj = Instantiate(trap.trapStats.projectile, spawnPos, transform.rotation) as GameObject;
        ProjectileMovement projectileMove = projectileObj.GetComponent<ProjectileMovement>();
        projectileMove.Fire(targetPos, trap.trapStats, trap.gameObject);
    }

    private void OnDestroy()
    {
        Deactivate();
    }

    private void OnDisable()
    {
        Deactivate();
    }
}
