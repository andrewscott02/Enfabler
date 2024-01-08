using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableHit : MonoBehaviour
{
    Rigidbody rb;
    Health health;

    public float forceMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            rb = GetComponentInChildren<Rigidbody>();

        health = GetComponent<Health>();
        health.HitReactionDelegate += OnHit;
    }

    public void OnHit(int damage, Vector3 dir)
    {
        rb.AddForce(dir * damage * forceMultiplier);
    }
}
