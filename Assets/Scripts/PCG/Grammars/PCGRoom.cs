using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class PCGRoom : MonoBehaviour
{
    public bool lockOverride = false;
    public bool lockDoor = false;
    public GameObject exit;

    public ObjectSpawner mainDoorPoint;
    public ObjectSpawner entranceDoorPoint;
    public ObjectSpawner defaultReverseRoomDoorPoint;

    public void TrySetMainDoorPoint(ObjectSpawner value)
    {
        if (mainDoorPoint == null)
            mainDoorPoint = value;
    }

    public Collider roomBounds;
    public LayerMask boundsLayer, enemyLayer;

    public ObjectSpawner[] doorPoints;
    public Volume localVolume, localVolumeNext;
    public Object tempSpawnerObjects;

    public Transform enemySpawners, objectSpawners;
    public Transform[] enemySpawnerChildren { get; private set; }
    public ObjectSpawner[] objectSpawnerChildren { get; private set; }

    public ThemeData theme { get; private set; }
    public ThemeData nextTheme { get; private set; }

    bool reversed = false;
    bool mainPath = false;
    int removedFromMainPath = 0;

    public List<PCGRoom> attachedRooms = new List<PCGRoom>();

    [ContextMenu("Show Debug")]
    public void SetupTransforms()
    {
        enemySpawnerChildren = new Transform[enemySpawners.childCount];
        for (int i = 0; i < enemySpawners.childCount; i++)
            enemySpawnerChildren[i] = enemySpawners.GetChild(i);

        objectSpawnerChildren = objectSpawners.GetComponentsInChildren<ObjectSpawner>();
    }

    public E_RoomTypes roomType { get; private set; }
    public GrammarsDungeonData dungeonData { get; private set; }

    public void Setup(E_RoomTypes roomType, GrammarsDungeonData dungeonData, ThemeData theme, ThemeData nextTheme, bool reversed, bool mainPath, int removedFromPath)
    {
        this.roomType = roomType;
        this.dungeonData = dungeonData;

        this.theme = theme;
        this.nextTheme = nextTheme;
        this.reversed = reversed;
        this.mainPath = mainPath;

        this.removedFromMainPath = removedFromPath;

        string mainPathString = mainPath ? " MAIN " : " SIDE ";

        name += " " + mainPathString + roomType.ToString() + " room (PCG) - " + theme.ToString();

        SetupTransforms();

        GenerateWalls();
        SetGenericMaterials();

        GetDoorPoints(reversed);

        if (mainPath)
            SpawnNextMainRoom();

        SpawnAdditionalRooms();
        SetupVolumes();
    }

    void GenerateWalls()
    {
        WallGenerator[] wallGenerators = GetComponentsInChildren<WallGenerator>();

        foreach (var item in wallGenerators)
        {
            if (!reversed)
                item.SetupRoom(item.changeTheme ? nextTheme : theme);
            else
                item.SetupRoom(item.changeTheme ? theme : nextTheme);
        }
    }

    void SetGenericMaterials()
    {
        SetMaterialPCG[] setMats = GetComponentsInChildren<SetMaterialPCG>();

        foreach (var item in setMats)
        {
            if (!reversed)
                item.Setup(item.changeTheme ? nextTheme : theme);
            else
                item.Setup(item.changeTheme ? theme : nextTheme);
        }
    }

    public bool Overlaps()
    {
        Collider[] cols = Physics.OverlapBox(roomBounds.bounds.center, roomBounds.bounds.extents, roomBounds.transform.rotation, boundsLayer);

        if (cols.Length > 1)
        {
            //Debug.Log("Destroying room, intersects with another");
            return true;
        }

        return false;
    }

    public int randomDoorPoint = 0;

    public void GetDoorPoints(bool reversedRoom)
    {
        doorPoints = exit.GetComponentsInChildren<ObjectSpawner>();

        if (reversedRoom)
        {
            int index = randomDoorPoint;
            ObjectSpawner newEntrance = doorPoints[index];

            if (defaultReverseRoomDoorPoint != null)
            {
                newEntrance = defaultReverseRoomDoorPoint;

                for (int i = 0; i < doorPoints.Length; i++)
                {
                    if (doorPoints[i] == newEntrance)
                        index = i;
                }
            }

            doorPoints[index] = entranceDoorPoint;
            entranceDoorPoint = newEntrance;

            ReverseTransform();
        }

        if (mainPath)
            mainDoorPoint = doorPoints[Random.Range(0, doorPoints.Length)];
    }

    void ReverseTransform()
    {
        //Debug.Log("Reverse room " + i);
        Vector3 rot = entranceDoorPoint.transform.rotation.eulerAngles + new Vector3(0, 180, 0);
        transform.rotation = Quaternion.Euler(rot);

        Vector3 offset = transform.position - entranceDoorPoint.transform.position;
        transform.position = transform.position + offset;

        //Vector3 rot = entranceDoorPoint.transform.rotation.eulerAngles + new Vector3(0, 180, 0);
        //transform.rotation = Quaternion.Euler(rot);
    }

    void SpawnNextMainRoom()
    {
        if (DungeonGenerator.instance.currentRoom >= DungeonGenerator.instance.rooms.Count)
            return;

        E_RoomTypes roomType = DungeonGenerator.instance.rooms[DungeonGenerator.instance.currentRoom];
        bool success = TrySpawnRoom(mainDoorPoint, roomType, true, 0);

        if (success)
        {
            SpawnDoors(mainDoorPoint, true);
            return;
        }

        //If no main room can be spawned, try another door point
        foreach (var item in doorPoints)
        {
            if (TrySpawnRoom(item, roomType, true, 0))
            {
                success = true;
                mainDoorPoint = item;
                SpawnDoors(item, true);
                return;
            }
        }

        //Tries next room, skipping current room data
        DungeonGenerator.instance.InsertEmptyRoom();
        SpawnNextMainRoom();
    }

    public void SpawnAdditionalRooms()
    {
        foreach (var item in doorPoints)
        {
            if (item != mainDoorPoint || mainDoorPoint == null)
            {
                //Debug.Log("Spawning side room");
                E_RoomTypes roomType = dungeonData.GetRandomRoomType();
                if (TrySpawnRoom(item, roomType, false, removedFromMainPath + 1))
                {
                    SpawnDoors(item, true);
                }
                else
                {
                    SpawnDoors(item, false);
                }
            }
        }
    }

    bool TrySpawnRoom(ObjectSpawner doorPoint, E_RoomTypes roomType, bool mainPath, int removedFromMainPath)
    {
        PCGRoom generatedRoom = DungeonGenerator.instance.GenerateRoom(roomType, doorPoint.changeTheme ^ reversed ? nextTheme : theme, doorPoint.transform, mainPath, removedFromMainPath);

        if (generatedRoom != null)
        {
            attachedRooms.Add(generatedRoom);
            generatedRoom.attachedRooms.Add(this);
            return true;
        }

        return false;
    }

    public ObjectSpawner GetRandomDoorPoint()
    {
        return doorPoints[Random.Range(0, doorPoints.Length)];
    }

    void SetupVolumes()
    {
        localVolume.profile = reversed ? nextTheme.volumeProfile : theme.volumeProfile;

        if (localVolumeNext != null)
            localVolumeNext.profile = reversed ? theme.volumeProfile : nextTheme.volumeProfile;
    }

    List<GameObject> itemsInRoom = new List<GameObject>();

    bool generated = false;

    public void PopulateRoom()
    {
        if (generated)
            return;

        //SpawnDoors();
        SpawnTraps();
        SpawnObjects();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.CompareTag("EndDoor") && transform.GetChild(i).gameObject.activeSelf)
                cullObjects.Add(transform.GetChild(i).gameObject);
        }

        //Invoke("SpawnEnemies", 0.5f);
        SpawnEnemies();
        SpawnBoss();

        SetPuzzleElements();

        generated = true;
    }

    Door mainDoor;
    List<Door> doors = new List<Door>();

    void SpawnDoors(ObjectSpawner doorSpawner, bool open)
    {
        ThemeData doorTheme = doorSpawner.changeTheme ^ reversed ? nextTheme : theme;
        GameObject go = Instantiate(open ? dungeonData.GetRandomDoor(doorTheme) : dungeonData.GetRandomDoorClosedOff(doorTheme), doorSpawner.transform) as GameObject;
        go.transform.position = doorSpawner.transform.position;
        go.transform.rotation = doorSpawner.transform.rotation;
        itemsInRoom.Add(go);

        Door door = go.GetComponentInChildren<Door>();
        door.lockedInteraction = lockOverride ? lockDoor : dungeonData.GetDoorLocked(roomType);

        door.interactDelegate += DoorOpened;

        doors.Add(door);

        if (doorSpawner == mainDoorPoint)
            mainDoor = door;
        //Debug.Log("Added delegate to room " + roomNumber);
    }

    bool doorOpenedLogic = false;

    public void DoorOpened()
    {
        //GrammarsDungeonGeneration.instance.CullRooms(roomNumber);

        if (doorOpenedLogic)
            return;

        doorOpenedLogic = true;

        //Debug.Log("Door opened - from delegate : Room " + roomNumber);
        //GrammarsDungeonGeneration.instance.PopulateRoom(roomNumber + GrammarsDungeonGeneration.instance.preloadRooms);

        if (nextTheme != theme)
            GameCanvasManager.instance.ShowRegionText(nextTheme.regionName);
    }

    int enemiesInRoom = 0;

    void SpawnEnemies()
    {
        int roomIndex = dungeonData.GetRoomDataIndex(roomType);
        roundsMax = dungeonData.roomData[roomIndex].enemySpawnInfo.Length;

        if (currentRound >= roundsMax)
            return;

        List<Object> enemiesToSpawn = dungeonData.GetRandomEnemies(roomType, theme, currentRound);

        foreach (var item in enemiesToSpawn)
        {
            int spawnerIndex = Random.Range(0, enemySpawnerChildren.Length);

            Vector3 spawnPos = enemySpawnerChildren[spawnerIndex].position;
            spawnPos.y += 1f;

            GameObject go = Instantiate(item, transform) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = Quaternion.identity;
            itemsInRoom.Add(go);

            BaseCharacterController enemy = go.GetComponent<BaseCharacterController>();
            enemy.characterDied += EnemyKilled;
            enemiesInRoom++;
        }
    }

    public void ForceRemoveEnemyFromRoom()
    {
        EnemyKilled(null);
    }

    public void EnemyKilled(BaseCharacterController controller)
    {
        enemiesInRoom--;

        if (enemiesInRoom <= nextRoundThreshold)
        {
            CheckRound();
        }

        if (enemiesInRoom <= 0)
        {
            //Debug.Log("Unlocking Doors");
            foreach (var item in doors)
                item.UnlockInteraction();
        }
    }

    int GetEnemiesSpawnedInRoom()
    {
        Collider[] cols = Physics.OverlapBox(roomBounds.bounds.center, roomBounds.bounds.extents, roomBounds.transform.rotation, enemyLayer);

        int enemies = 0;

        foreach (var item in cols)
        {
            BaseCharacterController enemy = item.gameObject.GetComponent<BaseCharacterController>();

            if (enemy != null)
            {
                if (enemy.checkedInRoomBounds)
                {
                    enemies++;
                }
            }
        }

        return enemies;
    }

    int currentRound = 0;
    int roundsMax;
    int nextRoundThreshold;

    bool CheckRound()
    {
        currentRound++;

        SpawnEnemies();

        return currentRound < roundsMax;
    }

    void SpawnTraps()
    {
        List<ObjectSpawnerInstance> objectsToSpawn = dungeonData.GetRandomTraps(this, theme, nextTheme);

        foreach (var item in objectsToSpawn)
        {
            Vector3 spawnPos = objectSpawnerChildren[item.spawnerIndex].transform.position;
            Quaternion spawnRot = objectSpawnerChildren[item.spawnerIndex].transform.rotation;

            GameObject go = Instantiate(item.objectPrefab, objectSpawnerChildren[item.spawnerIndex].transform) as GameObject;
            go.transform.position = spawnPos;
            go.transform.rotation = spawnRot;
            itemsInRoom.Add(go);
        }
    }

    void SpawnObjects()
    {
        System.Random r = new System.Random();
        foreach (int i in Enumerable.Range(0, objectSpawnerChildren.Length).OrderBy(x => r.Next()))
        {
            List<GameObject> generatedItems = objectSpawnerChildren[i].SpawnObject(objectSpawnerChildren[i].changeTheme ^ reversed ? nextTheme : theme, dungeonData);

            foreach (var generatedItem in generatedItems)
                itemsInRoom.Add(generatedItem);
        }

        dungeonData.ResetObjectData();
    }

    public bool GetValidSpawner(ObjectData objectData, out int spawnerIndex)
    {
        int startIndex = Random.Range(0, objectSpawnerChildren.Length);
        spawnerIndex = startIndex;

        while (true)
        {
            if (objectSpawnerChildren[spawnerIndex].transform.childCount == 0)
            {
                foreach (var item in objectData.validSpawnerTypes)
                {
                    if (item == objectSpawnerChildren[spawnerIndex].objectType)
                    {
                        //Creates temp game object to fill up the space
                        Instantiate(tempSpawnerObjects, objectSpawnerChildren[spawnerIndex].transform);
                        return true;
                    }
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
                item.interactable = mainDoor;
            }
        }
    }

    public List<GameObject> cullObjects = new List<GameObject>();
    bool culled = false;

    public void CullRoom(bool cull)
    {
        for (int i = 0; i < cullObjects.Count; i++)
        {
            if (cullObjects[i] != null)
                cullObjects[i].SetActive(!cull);
        }

        culled = cull;

        return;

        if (cull)
            CloseDoor();
    }

    public void CloseDoor()
    {
        foreach (var item in doors)
            item.CloseDoor();
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

        Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);

        if (enemySpawnerChildren != null)
        {
            foreach (var item in enemySpawnerChildren)
            {
                Gizmos.DrawSphere(item.position, 0.5f);
            }
        }
    }
}
