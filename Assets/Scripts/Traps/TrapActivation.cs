using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActivation : MonoBehaviour
{
    public LayerMask layerMask;
    Trap trap;

    private void Awake()
    {
        trap = GetComponentInParent<Trap>();
    }

    int colliding = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            if (!trap.trapStats.onlyHitPlayer || other.gameObject.CompareTag("Player"))
            {
                colliding++;
                CheckTrapActivation();
            }

            CheckTrapActivation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            if (!trap.trapStats.onlyHitPlayer || other.gameObject.CompareTag("Player"))
            {
                colliding--;
                CheckTrapActivation();
            }

            CheckTrapActivation();
        }
    }

    void CheckTrapActivation()
    {
        if (colliding > 0)
            trap.ActivateTrap();
        else
            trap.DeactivateTrap();
    }
}
