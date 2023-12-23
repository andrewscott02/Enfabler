using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGRoom : MonoBehaviour
{
    public bool lockOverride = false;
    public bool lockDoor = false;
    public ObjectSpawner doorPoint;
    public Object tempSpawnerObjects;

    public Transform enemySpawners, objectSpawners;
    Transform[] enemySpawnerChildren, objectSpawnerChildren;

    ThemeData theme, nextTheme;
    bool reversed = false;
    int roomNumber;

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

    public void Setup(E_RoomTypes roomType, GrammarsDungeonData dungeonData, ThemeData theme, ThemeData nextTheme, int index, bool reversed)
    {
        this.roomType = roomType;
        this.dungeonData = dungeonData;
        this.theme = theme;
        this.nextTheme = nextTheme;
        roomNumber = index;
        this.reversed = reversed;

        name += " " + roomType.ToString() + " room (PCG) - " + theme.ToString();

        SetupTransforms();

        WallGenerator[] wallGenerators = GetComponentsInChildren<WallGenerator>();

        foreach (var item in wallGenerators)
        {
            if (!reversed)
                item.SetupRoom(item.changeTheme ? nextTheme : theme);
            else
                item.SetupRoom(item.changeTheme ? theme : nextTheme);
        }
    }

    List<GameObject> itemsInRoom = new List<GameObject>();

    public void PopulateRoom()
    {
        SpawnDoor();
        SpawnTraps();
        SpawnObjects();

        Invoke("SpawnEnemies", 0.5f);
        SpawnBoss();

        SetPuzzleElements();
    }

    Door door;

    void SpawnDoor()
    {
        ThemeData doorTheme = doorPoint.changeTheme ? nextTheme : theme;
        GameObject go = Instantiate(dungeonData.GetRandomDoor(doorTheme), doorPoint.transform) as GameObject;
        go.transform.position = doorPoint.transform.position;
        go.transform.rotation = doorPoint.transform.rotation;
        itemsInRoom.Add(go);

        door = go.GetComponentInChildren<Door>();
        door.lockedInteraction = lockOverride ? lockDoor : dungeonData.GetDoorLocked(roomType);

        door.interactDelegate += DoorOpened;
        //Debug.Log("Added delegate to room " + roomNumber);
    }

    public void DoorOpened()
    {
        //Debug.Log("Door opened - from delegate : Room " + roomNumber);
        GrammarsDungeonGeneration.instance.PopulateRoom(roomNumber + GrammarsDungeonGeneration.instance.preloadRooms);

        if (nextTheme != theme)
            GameCanvasManager.instance.ShowRegionText(nextTheme.regionName);
    }

    int enemiesInRoom = 0;

    void SpawnEnemies()
    {
        int roomIndex = dungeonData.GetRoomDataIndex(roomType);
        int roundsMax = dungeonData.roomData[roomIndex].enemySpawnInfo.Length;

        if (currentRound >= roundsMax)
            return;

        List<Object> enemiesToSpawn = dungeonData.GetRandomEnemies(roomType, theme, currentRound);

        foreach (var item in enemiesToSpawn)
        {
            int spawnerIndex = Random.Range(0, enemySpawnerChildren.Length);

            Vector3 spawnPos = enemySpawnerChildren[spawnerIndex].position;
            spawnPos.y += 2f;

            if (HelperFunctions.GetRandomPoint(spawnPos, 7.5f, 1f, 100, out Vector3 point))
            {
                spawnPos = point;
            }

            GameObject go = Instantiate(item, transform) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = Quaternion.identity;
            itemsInRoom.Add(go);

            BaseCharacterController enemy = go.GetComponent<BaseCharacterController>();
            enemy.characterDied += EnemyKilled;
            enemiesInRoom++;
        }

        nextRoundThreshold = Mathf.RoundToInt((float)enemiesInRoom * 0.25f);
        if (nextRoundThreshold <= 0) nextRoundThreshold = 1;
    }

    int currentRound = 0;
    int nextRoundThreshold;

    void EnemyKilled(BaseCharacterController controller)
    {
        //Debug.Log("Enemy killed in room");
        enemiesInRoom--;

        if (enemiesInRoom <= nextRoundThreshold)
        {
            currentRound++;

            SpawnEnemies();
        }

        if (enemiesInRoom <= 0)
        {
            door.UnlockInteraction();
        }
    }

    void SpawnTraps()
    {
        List<ObjectSpawnerInstance> objectsToSpawn = dungeonData.GetRandomTraps(this, theme);

        foreach (var item in objectsToSpawn)
        {
            Vector3 spawnPos = objectSpawnerChildren[item.spawnerIndex].position;
            Quaternion spawnRot = objectSpawnerChildren[item.spawnerIndex].rotation;

            GameObject go = Instantiate(item.objectPrefab, objectSpawnerChildren[item.spawnerIndex]) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = spawnRot;
            itemsInRoom.Add(go);
        }
    }

    void SpawnObjects()
    {
        List<ObjectSpawnerInstance> objectsToSpawn = dungeonData.GetRandomObjects(this, theme);

        foreach (var item in objectsToSpawn)
        {
            Vector3 spawnPos = objectSpawnerChildren[item.spawnerIndex].position;
            Quaternion spawnRot = objectSpawnerChildren[item.spawnerIndex].rotation;

            GameObject go = Instantiate(item.objectPrefab, objectSpawnerChildren[item.spawnerIndex]) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = spawnRot;
            itemsInRoom.Add(go);
        }
    }

    public bool GetValidSpawner(ObjectData objectData, out int spawnerIndex)
    {
        int startIndex = Random.Range(0, objectSpawnerChildren.Length);
        spawnerIndex = startIndex;

        while (true)
        {
            if (objectSpawnerChildren[spawnerIndex].childCount == 0)
            {
                if (objectData.validSpawners == (objectData.validSpawners | (1 << objectSpawnerChildren[spawnerIndex].gameObject.layer)))
                {
                    //Creates temp game object to fill up the space
                    Instantiate(tempSpawnerObjects, objectSpawnerChildren[spawnerIndex]);
                    return true;
                }
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

        GameObject go = Instantiate(dungeonData.GetRandomBoss(theme), transform) as GameObject;
        go.transform.position = spawnPos;
        go.transform.rotation = Quaternion.identity;
        itemsInRoom.Add(go);

        BaseCharacterController enemy = go.GetComponent<BaseCharacterController>();
        enemy.characterDied += EnemyKilled;
        enemiesInRoom++;
    }

    void SetPuzzleElements()
    {
        if (roomType != E_RoomTypes.Puzzle) return;

        DoorLocks[] doorLocks = GetComponentsInChildren<DoorLocks>();

        foreach(var item in doorLocks)
        {
            if (item.unlockMainDoor)
            {
                item.interactable = door;
            }
        }
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
        Gizmos.DrawCube(transform.position + new Vector3(0, 2.3f, 0), new Vector3(4.75f, 4.6f, 1));

        if (objectSpawnerChildren != null)
        {
            foreach (var item in objectSpawnerChildren)
            {
                Gizmos.DrawSphere(item.position, 0.5f);

                Gizmos.DrawLine(item.position, item.position + (item.up * 2));
                Gizmos.DrawLine(item.position + (item.up * 2), (item.position + (item.up * 2)) + (item.right * 2));
            }
        }

        Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube(doorPoint.transform.position + new Vector3(0, 2.3f, 0), new Vector3(4.75f, 4.6f, 1));

        if (enemySpawnerChildren != null)
        {
            foreach (var item in enemySpawnerChildren)
            {
                Gizmos.DrawSphere(item.position, 0.5f);
            }
        }
    }
}
