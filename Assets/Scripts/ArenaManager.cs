using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public Object enemyPrefab;
    public int[] enemyCounts;
    public float interval = 30f;

    public float spawnRadius = 30f;

    int round = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ISpawnRounds());
    }

    IEnumerator ISpawnRounds()
    {
        yield return new WaitForSeconds(interval);
        SpawnEnemies();
        round++;
        StartCoroutine(ISpawnRounds());
    }

    void SpawnEnemies()
    {
        int enemiesToSpawn = enemyCounts[Mathf.Clamp(round, 0, enemyCounts.Length)];

        for(int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPos;
            if (!HelperFunctions.GetRandomPointOnNavmesh(transform.position, spawnRadius, 0.5f, 100, out spawnPos))
            {
                spawnPos = transform.position;
            }

            Instantiate(enemyPrefab, spawnPos, new Quaternion(0, 0, 0, 0));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
