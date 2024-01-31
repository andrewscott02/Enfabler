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
        if (end)
        {
            SpawnEndObject();
            return;
        }

        Object spawnObj = chainsToSpawn > 1 ? chainPrefab : chainEndPrefab;

        GameObject chainGO = Instantiate(spawnObj, chainSpawnTransform) as GameObject;
        endJoint.connectedBody = chainGO.GetComponent<Rigidbody>();

        ChainExtender extender = chainGO.GetComponentInChildren<ChainExtender>();
        extender.startJoint.connectedBody = GetComponent<Rigidbody>();

        chainsToSpawn -= 1;
        chainGO.GetComponentInChildren<ChainExtender>().SpawnChain(chainsToSpawn);
    }

    public void SpawnEndObject()
    {
        //TODO
    }
}
