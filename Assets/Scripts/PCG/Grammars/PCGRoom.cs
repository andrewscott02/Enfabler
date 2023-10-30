using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGRoom : MonoBehaviour
{
    public Transform exitPoint;

    public Transform enemySpawners, objectSpawners;
    Transform[] enemySpawnerChildren, objectSpawnerChildren;

    [ContextMenu("Show Debug")]
    public void SetupTransforms()
    {
        enemySpawnerChildren = new Transform[enemySpawners.childCount];
        for (int i = 0; i < enemySpawners.childCount; i++)
            enemySpawnerChildren[i] = enemySpawners.GetChild(i);

        objectSpawnerChildren = new Transform[objectSpawners.childCount];
        for (int i = 0; i < objectSpawners.childCount; i++)
            objectSpawnerChildren[i] = objectSpawners.GetChild(i);
    }

    public E_RoomTypes roomType { get; private set; }
    public GrammarsDungeonData dungeonData { get; private set; }

    public void Setup(E_RoomTypes roomType, GrammarsDungeonData dungeonData)
    {
        this.roomType = roomType;
        this.dungeonData = dungeonData;
        name += " " + roomType.ToString() + " room (PCG)";

        SetupTransforms();
    }

    List<GameObject> itemsInRoom = new List<GameObject>();

    public void PopulateRoom()
    {
        SpawnEnemies();
        SpawnTraps();
        SpawnBoss();
    }

    void SpawnEnemies()
    {
        int enemiesToSpawn = dungeonData.GetEnemyCount(roomType);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Debug.Log("Spawning enemy in room - " + name);

            int spawnerIndex = Random.Range(0, enemySpawnerChildren.Length);

            Vector3 spawnPos = enemySpawnerChildren[spawnerIndex].position;

            GameObject go = Instantiate(dungeonData.GetRandomEnemy(), transform) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = Quaternion.identity;
            itemsInRoom.Add(go);
        }
    }

    void SpawnTraps()
    {
        int trapsToSpawn = dungeonData.GetTrapCount(roomType);

        for (int i = 0; i < trapsToSpawn; i++)
        {
            //TODO check valid spawners

            Debug.Log("Spawning trap in room - " + name);

            int spawnerIndex = Random.Range(0, objectSpawnerChildren.Length);

            Vector3 spawnPos = objectSpawnerChildren[spawnerIndex].position;

            GameObject go = Instantiate(dungeonData.GetRandomTrap(), transform) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = Quaternion.identity;
            itemsInRoom.Add(go);
        }
    }

    void SpawnBoss()
    {
        if (roomType != E_RoomTypes.Boss) return;

        Debug.Log("Spawning enemy in room - " + name);

        int spawnerIndex = Random.Range(0, enemySpawnerChildren.Length);

        Vector3 spawnPos = enemySpawnerChildren[spawnerIndex].position;

        GameObject go = Instantiate(dungeonData.GetRandomBoss(), transform) as GameObject;
        go.transform.position = spawnPos;
        go.transform.rotation = Quaternion.identity;
        itemsInRoom.Add(go);
    }

    public void DeleteRoom()
    {
        for (int i = 0; i < itemsInRoom.Count; i++)
        {
            DestroyImmediate(itemsInRoom[i]);
        }
    }

    public bool resetTransformsGizmo = false;

    private void OnDrawGizmos()
    {
        if (resetTransformsGizmo)
            SetupTransforms();

        Gizmos.color = Color.blue - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position + new Vector3(0, 2f, 0), new Vector3(2, 3, 1));

        if (objectSpawnerChildren != null)
        {
            foreach (var item in objectSpawnerChildren)
            {
                Gizmos.DrawSphere(item.position, 0.5f);
            }
        }

        Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube(exitPoint.position + new Vector3(0, 2f, 0), new Vector3(2, 3, 1));

        if (enemySpawnerChildren != null)
        {
            foreach (var item in enemySpawnerChildren)
            {
                Gizmos.DrawSphere(item.position, 0.5f);
            }
        }
    }
}
