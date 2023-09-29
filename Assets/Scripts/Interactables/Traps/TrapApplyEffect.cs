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
        switch (trap.trapStats.targetType)
        {
            case E_TargetType.Shot:
                //TODO: line trace to target
                break;
            case E_TargetType.Area:
                hitTargets = new List<IDamageable>();
                effectCollider.enabled = true;
                StartCoroutine(IDelayDeactivate(0.1f));
                break;
                break;
            default:
                break;
        }
    }

    public void Deactivate()
    {
        hitTargets = new List<IDamageable>();
        effectCollider.enabled = false;
    }

    IEnumerator IDelayDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        Deactivate();
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
