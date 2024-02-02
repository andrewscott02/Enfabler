using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPuddleSpawner : MonoBehaviour
{
    public LayerMask layerMask;
    public Object waterPuddlePrefab;

    public Vector3 maxScaleModifier;

    public float spawnChance = 0.5f;

    void Start()
    {
        if (Random.Range(0f, 1f) <= spawnChance)
            Invoke("SpawnPuddle", 0.5f);
    }

    void SpawnPuddle()
    {
        RaycastHit hit;
        if (Physics.Linecast(start: transform.position, end: transform.position + (Vector3.down * 15f), hitInfo: out hit, layerMask))
        {
            GameObject waterGO = Instantiate(waterPuddlePrefab, hit.point, Quaternion.Euler(hit.normal)) as GameObject;

            Vector3 scale = new Vector3();
            scale.x = Random.Range(waterGO.transform.localScale.x, waterGO.transform.localScale.x * maxScaleModifier.x);
            scale.y = Random.Range(waterGO.transform.localScale.y, waterGO.transform.localScale.y * maxScaleModifier.y);
            scale.z = Random.Range(waterGO.transform.localScale.z, waterGO.transform.localScale.z * maxScaleModifier.z);

            waterGO.transform.localScale = scale;
        }
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
