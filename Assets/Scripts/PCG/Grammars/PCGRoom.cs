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
        SpawnDoor();
        SpawnEnemies();
        SpawnTraps();
        SpawnObjects();
        SpawnBoss();
    }

    Door door;

    void SpawnDoor()
    {
        GameObject go = Instantiate(dungeonData.GetRandomDoor(), exitPoint) as GameObject;
        go.transform.position = exitPoint.transform.position;
        go.transform.rotation = exitPoint.transform.rotation;
        itemsInRoom.Add(go);

        Door interactable = go.GetComponent<Door>();
        interactable.lockedInteraction = dungeonData.GetDoorLocked(roomType);
        door = interactable;
    }

    int enemiesInRoom = 0;

    void SpawnEnemies()
    {
        int enemiesToSpawn = dungeonData.GetEnemyCount(roomType);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            //Debug.Log("Spawning enemy in room - " + name);

            int spawnerIndex = Random.Range(0, enemySpawnerChildren.Length);

            Vector3 spawnPos = enemySpawnerChildren[spawnerIndex].position;

            GameObject go = Instantiate(dungeonData.GetRandomEnemy(), transform) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = Quaternion.identity;
            itemsInRoom.Add(go);

            BaseCharacterController enemy = go.GetComponent<BaseCharacterController>();
            enemy.characterDied += EnemyKilled;
            enemiesInRoom++;
        }
    }

    void EnemyKilled(BaseCharacterController controller)
    {
        //Debug.Log("Enemy killed in room");
        enemiesInRoom--;

        if (enemiesInRoom <= 0)
        {
            door.UnlockInteraction();
        }
    }

    void SpawnTraps()
    {
        int trapsToSpawn = dungeonData.GetTrapCount(roomType);

        for (int i = 0; i < trapsToSpawn; i++)
        {
            //Debug.Log("Spawning trap in room - " + name);

            if (!GetValidSpawner(out int spawnerIndex)) return;

            Vector3 spawnPos = objectSpawnerChildren[spawnerIndex].position;

            GameObject go = Instantiate(dungeonData.GetRandomTrap(), objectSpawnerChildren[spawnerIndex]) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = Quaternion.identity;
            itemsInRoom.Add(go);
        }
    }

    void SpawnObjects()
    {
        int objectsToSpawn = 10;

        for (int i = 0; i < objectsToSpawn; i++)
        {
            //Debug.Log("Spawning objects in room - " + name);

            if (!GetValidSpawner(out int spawnerIndex)) return;

            Vector3 spawnPos = objectSpawnerChildren[spawnerIndex].position;

            GameObject go = Instantiate(dungeonData.GetRandomObject(), objectSpawnerChildren[spawnerIndex]) as GameObject;
            go.transform.position = spawnPos;
            Quaternion spawnRot = Quaternion.identity;
            spawnRot.y = Random.Range(0, 360);
            go.transform.rotation = spawnRot;
            itemsInRoom.Add(go);
        }
    }

    bool GetValidSpawner(out int spawnerIndex)
    {
        int startIndex = Random.Range(0, objectSpawnerChildren.Length);
        spawnerIndex = startIndex;

        while (true)
        {
            if (objectSpawnerChildren[spawnerIndex].childCount == 0)
            {
                return true;
            }

            spawnerIndex++;

            if (spawnerIndex >= objectSpawnerChildren.Length)
                spawnerIndex = 0;

            if (spawnerIndex == startIndex)
                return false;
        }
    }

    void SpawnBoss()
    {
        if (roomType != E_RoomTypes.Boss) return;

        //Debug.Log("Spawning enemy in room - " + name);

        int spawnerIndex = Random.Range(0, enemySpawnerChildren.Length);

        Vector3 spawnPos = enemySpawnerChildren[spawnerIndex].position;

        GameObject go = Instantiate(dungeonData.GetRandomBoss(), transform) as GameObject;
        go.transform.position = spawnPos;
        go.transform.rotation = Quaternion.identity;
        itemsInRoom.Add(go);

        BaseCharacterController enemy = go.GetComponent<BaseCharacterController>();
        enemy.characterDied += EnemyKilled;
        enemiesInRoom++;
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
