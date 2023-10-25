using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public ArenaFight arenaFight;
    public float interval = 5f;
    public float spawnRadius = 30f;

    int round = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ISpawnRounds(interval));
        AIManager.instance.enemiesDied += StartRoundCoroutine;
    }

    void StartRoundCoroutine()
    {
        StartCoroutine(ISpawnRounds(interval));
    }

    IEnumerator ISpawnRounds(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEnemies();
        round = Mathf.Clamp(round + 1, 0, arenaFight.arenaRounds.Length - 1);
    }

    void SpawnEnemies()
    {
        foreach (var item in arenaFight.arenaRounds[round].enemyTypes)
        {
            for (int i = 0; i < item.count; i++)
            {
                Vector3 spawnPos;
                if (!HelperFunctions.GetRandomPointOnNavmesh(transform.position, spawnRadius, 0.5f, 100, out spawnPos))
                {
                    spawnPos = transform.position;
                }

                Instantiate(item.enemyObject, spawnPos, new Quaternion(0, 0, 0, 0));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
