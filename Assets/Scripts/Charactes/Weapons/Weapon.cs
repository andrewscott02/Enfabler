using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject trail;
    public GameObject weaponBase, weaponTip;
    public GameObject weaponBaseHit, weaponTipHit;

    Collider col;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        trail.SetActive(false);
    }

    public void Disarm()
    {
        col.enabled = true;
        rb.isKinematic = false;
        transform.parent = null;
    }
}
