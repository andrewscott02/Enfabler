using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitKnockback : MonoBehaviour
{
    Rigidbody rb;
    public float hitKnockBack = 300f;
    public float parryKnockback = 1000f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<Health>().HitReactionDelegate += Hit;
        GetComponent<CharacterCombat>().hitParry += Parry;
    }

    public void Hit(int damage, Vector3 dir)
    {
        rb.AddForce(dir * hitKnockBack * damage, ForceMode.Impulse);
    }

    public void Parry(Vector3 dir)
    {
        Debug.Log("Parry Knockback on " + gameObject.name);
        rb.AddForce(dir * parryKnockback, ForceMode.Impulse);
    }
}
