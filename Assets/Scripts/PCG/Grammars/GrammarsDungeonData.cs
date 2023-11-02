using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsDungeonData", menuName = "PCG/Grammars/DungeonData", order = 0)]
public class GrammarsDungeonData : ScriptableObject
{
    public RoomData[] roomData;
    public E_RoomTypes[] additionalRoomTypes;
    public int additionalHealingRooms = 0;
    Dictionary<E_RoomTypes, int> roomDict;
    public Vector2Int roomsCountMinMax;

    public EnemyData[] enemies;
    public Object[] doors, traps, objects, bosses;

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
                Debug.Log("Couldn't find valid room, returning random");
                return additionalRoomTypes[currentIndex];
            }
        }
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

    #region Prefabs

    public Object GetRandomRoomPrefab(E_RoomTypes roomType)
    {
        int index = GetRoomDataIndex(roomType);

        if (index >= 0 && index < roomData.Length)
        {
            return DeterminePrefab(roomData[index].prefabData);
        }

        return null;
    }

    Object DeterminePrefab(RoomPrefabData[] prefabData)
    {
        int startIndex = Random.Range(0, prefabData.Length);
        int currentIndex = startIndex;

        while (true)
        {
            if (prefabData[currentIndex].CanUse())
            {
                prefabData[currentIndex].Used();
                return prefabData[currentIndex].GetRandomPrefab();
            }

            currentIndex++;

            if (currentIndex >= prefabData.Length)
                currentIndex = 0;

            if (currentIndex == startIndex)
                return null;
        }
    }

    public Object GetRandomDoor()
    {
        return doors[Random.Range(0, doors.Length)];
    }

    public List<Object> GetRandomEnemies(E_RoomTypes roomType)
    {
        List<Object> enemiesToAdd = new List<Object>();

        int index = GetRoomDataIndex(roomType);
        int budget = roomData[index].enemiesSeverityMax;

        bool budgetLeft = budget > 0;

        int totalEnemies = 0;

        while (budgetLeft)
        {
            if (GetRandomEnemy(out int enemyIndex, budget))
            {
                enemiesToAdd.Add(enemies[enemyIndex].enemyPrefab);
                budget -= enemies[enemyIndex].severity;
                totalEnemies++;
            }
            else
            {
                budgetLeft = false;
            }

            if (totalEnemies >= roomData[index].enemiesMax)
                budgetLeft = false;
        }

        ResetEnemyData();

        return enemiesToAdd;
    }

    void ResetEnemyData()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].timesUsed = 0;
        }
    }

    bool GetRandomEnemy(out int enemyIndex, int budgetLeft)
    {
        int startIndex = Random.Range(0, enemies.Length);
        enemyIndex = startIndex;

        while (true)
        {
            if (enemies[enemyIndex].severity <= budgetLeft && enemies[enemyIndex].timesUsed < enemies[enemyIndex].maxCount)
            {
                enemies[enemyIndex].timesUsed++;
                return true;
            }

            enemyIndex++;

            if (enemyIndex >= enemies.Length)
                enemyIndex = 0;

            if (enemyIndex == startIndex)
                return false;
        }
    }

    public Object GetRandomTrap()
    {
        return traps[Random.Range(0, traps.Length)];
    }

    public Object GetRandomObject()
    {
        return objects[Random.Range(0, objects.Length)];
    }

    public Object GetRandomBoss()
    {
        return bosses[Random.Range(0, bosses.Length)];
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

    public int enemiesMax;
    public int enemiesSeverityMax;

    public Vector2Int trapsMinMax;
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
public enum E_RoomTypes
{
    Start, Boss, End,
    Encounter, Puzzle, Treasure, Healing, Trap
}

[System.Serializable]
public enum E_RoomPrefabTypes
{
    Room, WideRoom, Corridor, Stairway, Grandstairway
}