using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActivation : MonoBehaviour
{
    Trap trap;

    private void Awake()
    {
        trap = GetComponentInParent<Trap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger with " + other.gameObject.name);
            trap.ActivateTrap();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger with " + other.gameObject.name);
            trap.DeactivateTrap();
        }
    }
}
