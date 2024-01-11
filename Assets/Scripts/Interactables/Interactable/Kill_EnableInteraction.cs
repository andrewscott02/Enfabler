using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill_EnableInteraction : MonoBehaviour
{
    public Interactable interactable;

    // Start is called before the first frame update
    void Start()
    {
        Health health = GetComponent<Health>();
        health.killDelegate += OnKill;
    }

    public void OnKill(Vector3 attacker, int damage)
    {
        interactable.UnlockInteraction();
    }
}
