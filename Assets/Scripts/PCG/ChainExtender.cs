using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainExtender : MonoBehaviour
{
    public Transform chainSpawnTransform;
    public Object chainPrefab, chainEndPrefab;

    public Joint startJoint, endJoint;

    public bool end = false;

    public void SpawnChain(int chainsToSpawn)
    {
        if (end || chainsToSpawn <= 0)
        {
            SpawnEndObject();
            return;
        }

        Object spawnObj = chainsToSpawn > 1 ? chainPrefab : chainEndPrefab;

        GameObject chainGO = Instantiate(spawnObj, chainSpawnTransform) as GameObject;
        ChainExtender extender = chainGO.GetComponentInChildren<ChainExtender>();

        endJoint.connectedBody = extender.GetComponent<Rigidbody>();
        extender.startJoint.connectedBody = endJoint.GetComponent<Rigidbody>();

        chainsToSpawn -= 1;
        chainGO.GetComponentInChildren<ChainExtender>().SpawnChain(chainsToSpawn);
    }

    public void SpawnEndObject()
    {
        //TODO
    }
}
