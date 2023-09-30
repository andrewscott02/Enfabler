using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHit : MonoBehaviour
{
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

            if (hitDamageable == trap.GetComponent<IDamageable>())
            {
                return;
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

        Debug.Log("hitting target for " + trap.trapStats.damage);
        if (trap.trapStats.shotAOE == 0)
        {
            //only hit target
            MonoBehaviour targetMono = target as MonoBehaviour;
            trap.ApplyEffect(target, targetMono.gameObject.transform.position);
        }
        else
        {
            //TODO: Affect AOE targets
            //Spawn impulse
        }

        Destroy(move.gameObject);
    }
}
