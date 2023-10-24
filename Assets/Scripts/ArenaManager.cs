using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [System.Serializable]
    public struct ArenaRound
    {
        public ArenaRoundEnemies[] enemyTypes;
        public int interval;
    }

    [System.Serializable]
    public struct ArenaRoundEnemies
    {
        public Object enemyObject;
        public int count;
    }

    public ArenaRound[] arenaRounds;

    public float spawnRadius = 30f;

    int round = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ISpawnRounds());
    }

    IEnumerator ISpawnRounds()
    {
        yield return new WaitForSeconds(arenaRounds[round].interval);
        SpawnEnemies();
        round = Mathf.Clamp(round + 1, 0, arenaRounds.Length);
        StartCoroutine(ISpawnRounds());
    }

    void SpawnEnemies()
    {
        foreach (var item in arenaRounds[round].enemyTypes)
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
