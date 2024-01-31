using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBase : ChainExtender
{
    public LayerMask layerMask;
    public float chainPieceLength = 1.25f;

    protected void Start()
    {
        Invoke("StartSpawningChains", 0.5f);
    }

    void StartSpawningChains()
    {
        int maxChains = 3;

        RaycastHit hit;
        if (Physics.Linecast(start: transform.position, end: transform.position + (Vector3.down * 15f), hitInfo: out hit, layerMask))
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            maxChains = (int)(distance / chainPieceLength);
        }

        int chainsToSpawn = Random.Range(0, maxChains);
        SpawnChain(chainsToSpawn);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        RaycastHit hit;
        if (Physics.Linecast(start: transform.position, end: transform.position + (Vector3.down * 15f), hitInfo: out hit, layerMask))
        {
            Gizmos.DrawLine(transform.position, hit.point);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hit.point, transform.position + (Vector3.down * 15f));
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * 15f));
        }
    }
}
