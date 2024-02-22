using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsDungeonData", menuName = "PCG/Grammars/DungeonData", order = 0)]
public class GrammarsDungeonData : ScriptableObject
{
    #region Variables

    public RoomData[] roomData;
    public E_RoomTypes[] additionalRoomTypes;
    public int additionalHealingRooms = 0;
    Dictionary<E_RoomTypes, int> roomDict;
    public Vector2Int roomsCountMinMax;

    public List<ThemeData> startingThemes;
    public List<ThemeData> allThemes;
    public Vector2Int themeChanges;

    public bool allowDuplicates = false;

    #endregion

    #region Room Generation

    public void ResetAllDungeonData()
    {
        roomDict = new Dictionary<E_RoomTypes, int>();

        foreach(var item in additionalRoomTypes)
        {
            roomDict.Add(item, 0);
        }

        foreach(var item in roomData)
        {
            foreach (var data in item.prefabData)
            {
                data.ResetData();
            }
        }

        ResetEnemyData();
    }
    
    public E_RoomTypes GetRandomRoomType()
    {
        int startIndex = Random.Range(0, additionalRoomTypes.Length);
        int currentIndex = startIndex;

        while (true)
        {
            if (roomDict.ContainsKey(additionalRoomTypes[currentIndex]))
            {
                int index = GetRoomDataIndex(additionalRoomTypes[currentIndex]);
                if (roomDict[additionalRoomTypes[currentIndex]] < roomData[index].countMinMax.y)
                {
                    roomDict[additionalRoomTypes[currentIndex]]++;
                    return additionalRoomTypes[currentIndex];
                }
            }

            currentIndex++;

            if (currentIndex >= additionalRoomTypes.Length)
                currentIndex = 0;

            if (currentIndex == startIndex)
            {
                //Debug.Log("Couldn't find valid room, returning random");
                return additionalRoomTypes[currentIndex];
            }
        }
    }

    public E_RoomTypes GetRandomRoomTypeIgnoreLimits()
    {
        int startIndex = Random.Range(0, additionalRoomTypes.Length);
        return additionalRoomTypes[startIndex];
    }

    public int GetRoomDataIndex(E_RoomTypes roomType)
    {
        for (int i = 0; i < roomData.Length; i++)
        {
            if (roomData[i].roomType == roomType)
            {
                return i;
            }
        }

        return -1;
    }

    #endregion

    #region Prefabs

    #region Room

    public Object GetRandomRoomPrefab(E_RoomTypes roomType, ThemeData currentTheme, out ThemeData nextRoomTheme, out bool reversed)
    {
        int index = GetRoomDataIndex(roomType);
        nextRoomTheme = currentTheme;
        reversed = false;

        if (index >= 0 && index < roomData.Length)
        {
            Object prefab = DeterminePrefab(roomData[index].prefabData, currentTheme, out nextRoomTheme, out reversed);

            return prefab;
        }

        return null;
    }

    Object DeterminePrefab(RoomPrefabData[] prefabData, ThemeData currentTheme, out ThemeData nextRoomTheme, out bool reversed)
    {
        nextRoomTheme = currentTheme;
        int startIndex = Random.Range(0, prefabData.Length);
        int currentIndex = startIndex;
        reversed = false;

        while (true)
        {
            if (prefabData[currentIndex].CanUse(currentTheme))
            {
                prefabData[currentIndex].Used();
                nextRoomTheme = prefabData[currentIndex].GetNextRoomTheme(currentTheme);
                reversed = prefabData[currentIndex].reverseRooms;
                return prefabData[currentIndex].GetRandomPrefab();
            }

            currentIndex++;

            if (currentIndex >= prefabData.Length)
                currentIndex = 0;

            if (currentIndex == startIndex)
                return null;
        }
    }

    public Object GetRandomDoor(ThemeData theme)
    {
        return theme.doors[Random.Range(0, theme.doors.Length)];
    }

    #endregion

    #region Enemies

    public List<Object> GetRandomEnemies(E_RoomTypes roomType, ThemeData theme, int round)
    {
        List<Object> enemiesToAdd = new List<Object>();

        int index = GetRoomDataIndex(roomType);
        int budget = roomData[index].enemySpawnInfo[round].enemiesSeverityMax;

        bool budgetLeft = budget > 0;

        int totalEnemies = 0;

        while (budgetLeft)
        {
            if (GetRandomEnemy(out int enemyIndex, budget, theme))
            {
                enemiesToAdd.Add(theme.enemies[enemyIndex].enemyPrefab);
                budget -= theme.enemies[enemyIndex].severity;
                totalEnemies++;
            }
            else
            {
                budgetLeft = false;
            }

            if (totalEnemies >= roomData[index].enemySpawnInfo[round].enemiesMax)
                budgetLeft = false;
        }

        ResetEnemyData();

        return enemiesToAdd;
    }

    void ResetEnemyData()
    {
        foreach (var item in allThemes)
        {
            for (int i = 0; i < item.enemies.Length; i++)
            {
                item.enemies[i].timesUsed = 0;
            }
        }
    }

    bool GetRandomEnemy(out int enemyIndex, int budgetLeft, ThemeData theme)
    {
        int startIndex = Random.Range(0, theme.enemies.Length);
        enemyIndex = startIndex;

        while (true)
        {
            if (theme.enemies[enemyIndex].severity <= budgetLeft && theme.enemies[enemyIndex].timesUsed < theme.enemies[enemyIndex].maxCount)
            {
                theme.enemies[enemyIndex].timesUsed++;
                return true;
            }

            enemyIndex++;

            if (enemyIndex >= theme.enemies.Length)
                enemyIndex = 0;

            if (enemyIndex == startIndex)
                return false;
        }
    }

    #endregion

    #region Traps

    public List<ObjectSpawnerInstance> GetRandomTraps(PCGRoom room, ThemeData theme, ThemeData nextTheme)
    {
        List<ObjectSpawnerInstance> objectsToAdd = new List<ObjectSpawnerInstance>();

        int index = GetRoomDataIndex(room.roomType);
        int count = Random.Range(roomData[index].trapsMinMax.x, roomData[index].trapsMinMax.y);

        bool generating = true;
        int currentCount = 0;

        if (count == 0)
            return objectsToAdd;

        while (generating)
        {
            if (GetRandomTrap(room, out int trapIndex, out int spawnerIndex, theme, nextTheme))
            {
                ObjectSpawnerInstance instance = new ObjectSpawnerInstance();
                instance.objectPrefab = theme.traps[trapIndex].objectPrefab;
                instance.spawnerIndex = spawnerIndex;

                objectsToAdd.Add(instance);
                currentCount++;
            }
            else
            {
                generating = false;
            }

            if (currentCount >= count)
                generating = false;
        }

        ResetTrapData();

        return objectsToAdd;
    }

    void ResetTrapData()
    {
        foreach (var item in allThemes)
        {
            for (int i = 0; i < item.traps.Length; i++)
            {
                item.traps[i].timesUsed = 0;
            }
        }
    }

    bool GetRandomTrap(PCGRoom room, out int trapIndex, out int spawnerIndex, ThemeData theme, ThemeData nextTheme)
    {
        int startIndex = Random.Range(0, theme.traps.Length);
        trapIndex = startIndex;
        spawnerIndex = 0;

        while (true)
        {
            if (theme.traps[trapIndex].timesUsed < theme.traps[trapIndex].maxCount)
            {
                if (room.GetValidSpawner(theme.traps[trapIndex], out spawnerIndex))
                {
                    theme.traps[trapIndex].timesUsed++;
                    return true;
                }
            }

            trapIndex++;

            if (trapIndex >= theme.traps.Length)
                trapIndex = 0;

            if (trapIndex == startIndex)
                return false;
        }
    }

    #endregion

    #region Objects

    public bool GetRandomObject(ObjectSpawner spawner, out int objectIndex, ThemeData theme, bool trap)
    {
        if (TryGetRandomObject(spawner, out objectIndex, theme, trap))
        {
            return true;
        }

        int startIndex = Random.Range(0, trap ? theme.traps.Length : theme.objects.Length);
        objectIndex = startIndex;

        while (true)
        {
            if (CanUseObject(trap ? theme.traps[objectIndex] : theme.objects[objectIndex], spawner))
            {
                if (trap)
                    theme.traps[objectIndex].timesUsed++;
                else
                    theme.objects[objectIndex].timesUsed++;
                return true;
            }

            objectIndex++;

            if (trap ? objectIndex >= theme.traps.Length : objectIndex >= theme.objects.Length)
                objectIndex = 0;

            if (objectIndex == startIndex)
                return false;
        }
    }

    public bool TryGetRandomObject(ObjectSpawner spawner, out int objectIndex, ThemeData theme, bool trap)
    {
        objectIndex = 0;

        for (int i = 0; i <= 50; i ++)
        {
            objectIndex = Random.Range(0, trap ? theme.traps.Length : theme.objects.Length);

            if (CanUseObject(trap ? theme.traps[objectIndex] : theme.objects[objectIndex], spawner))
            {
                if (trap)
                    theme.traps[objectIndex].timesUsed++;
                else
                    theme.objects[objectIndex].timesUsed++;
                return true;
            }
        }

        return false;
    }

    bool CanUseObject(ObjectData data, ObjectSpawner spawner)
    {
        if (data.timesUsed >= data.maxCount)
            return false;

        foreach (var item in data.validSpawnerTypes)
        {
            if (spawner.objectType == item)
            {
                return true;
            }
        }

        return false;
    }

    public void ResetObjectData()
    {
        foreach (var item in allThemes)
        {
            for (int i = 0; i < item.objects.Length; i++)
            {
                item.objects[i].timesUsed = 0;
            }
        }
    }

    #endregion

    public Object GetRandomBoss(ThemeData theme)
    {
        return theme.bosses[Random.Range(0, theme.bosses.Length)];
    }

    public bool GetDoorLocked(E_RoomTypes roomType)
    {
        int index = GetRoomDataIndex(roomType);

        if (index >= 0 && index < roomData.Length)
        {
            return roomData[index].lockDoor;
        }

        return false;
    }

    public int GetTrapCount(E_RoomTypes roomType)
    {
        int index = GetRoomDataIndex(roomType);

        if (index >= 0 && index < roomData.Length)
        {
            return Random.Range(roomData[index].trapsMinMax.x, roomData[index].trapsMinMax.y + 1);
        }

        return 0;
    }

    #endregion

    #region Rules

    public List<E_RoomTypes> ReplaceDuplicates(List<E_RoomTypes> rooms)
    {
        if (allowDuplicates) return rooms;

        bool changed = true;

        while (changed)
        {
            ReplaceDuplicatesRecursive(rooms, out changed);
        }

        return rooms;
    }

    List<E_RoomTypes> ReplaceDuplicatesRecursive(List<E_RoomTypes> rooms, out bool changed)
    {
        changed = false;

        for (int i = 0; i < rooms.Count; i++)
        {
            if (i - 1 >= 0)
            {
                //If this room is the same as the previous room, replace it with a new one
                if (rooms[i].ToString() == rooms[i - 1].ToString())
                {
                    changed = true;
                    rooms[i] = GetRandomRoomType();
                }
            }
        };

        return rooms;
    }

    public List<E_RoomTypes> EnsureMinimums(List<E_RoomTypes> rooms)
    {
        for (int i = 0; i < roomData.Length; i++)
        {
            int count = 0;

            foreach (var item in rooms)
            {
                if (item == roomData[i].roomType)
                    count++;
            }

            int diff = roomData[i].countMinMax.x - count;

            for (int x = 0; x < diff; x++)
            {
                rooms.Add(roomData[i].roomType);
            }
        }

        return rooms;
    }

    #endregion
}

[System.Serializable]
public struct RoomData
{
    public E_RoomTypes roomType;
    public RoomPrefabData[] prefabData;
    public Vector2Int countMinMax;

    public bool lockDoor;

    public RoundData[] enemySpawnInfo;

    public Vector2Int trapsMinMax;
}

[System.Serializable]
public struct RoundData
{
    public int enemiesMax;
    public int enemiesSeverityMax;
}

[System.Serializable]
public enum E_RoomTypes
{
    Start, Boss, End,
    Encounter, Puzzle, Treasure, Healing, Trap, ChangeTheme, Arena
}

[System.Serializable]
public enum E_RoomPrefabTypes
{
    Room, WideRoom, Corridor, Stairway, Grandstairway
}

[System.Serializable]
public struct EnemyData
{
    public Object enemyPrefab;
    public int severity;
    public int maxCount;

    [HideInInspector]
    public int timesUsed;
}

[System.Serializable]
public struct ObjectData
{
    public Object objectPrefab;
    public bool canLink;
    public E_ObjectSpawnTypes[] validSpawnerTypes;
    public float randomPositiont;
    public bool randomRotation;
    public Vector3 randomRotationAxes;
    public int maxCount;

    [HideInInspector]
    public int timesUsed;
}

public struct ObjectSpawnerInstance
{
    public Object objectPrefab;
    public int spawnerIndex;
}