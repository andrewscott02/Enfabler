using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnDeath : MonoBehaviour
{
    bool beingDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<Health>().killDelegate += DestroyThis;
    }

    private void OnDestroy()
    {
        beingDestroyed = true;
    }

    void DestroyThis(Vector3 origin, int damage)
    {
        if (beingDestroyed) return;
        Destroy(this.gameObject);
    }
}
