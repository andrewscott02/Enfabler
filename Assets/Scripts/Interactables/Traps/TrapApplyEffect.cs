using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapApplyEffect : MonoBehaviour
{
    Trap trap;
    Collider effectCollider;

    private void Awake()
    {
        trap = GetComponentInParent<Trap>();
        effectCollider = GetComponent<Collider>();
    }

    public void Activate()
    {
        hitTargets = new List<IDamageable>();
        effectCollider.enabled = true;
        Invoke("Deactivate", 0.1f);
    }

    public void Deactivate()
    {
        hitTargets = new List<IDamageable>();
        effectCollider.enabled = false;
    }

    public LayerMask layerMask;
    public List<IDamageable> hitTargets;

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

            //Return if it has already been hit or if it should be ignored
            if (hitTargets.Contains(hitDamageable))
            {
                Debug.LogWarning("Ignore");
                return;
            }

            #endregion

            //If it can be hit, deal damage to target and add it to the hit targets list
            hitTargets.Add(hitDamageable);
            trap.ApplyEffect(hitDamageable, other.gameObject.transform.position);
        }
    }
}
