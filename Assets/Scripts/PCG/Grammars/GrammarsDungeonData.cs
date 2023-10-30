using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsDungeonData", menuName = "PCG/Grammars/DungeonData", order = 0)]
public class GrammarsDungeonData : ScriptableObject
{
    public RoomData[] roomData;
    public E_RoomTypes[] additionalRoomTypes;
    public Vector2Int roomsCountMinMax;

    public Object[] enemies, traps, objects;
    public Vector2Int enemiesPerRoom;

    public void ResetAllDungeonData()
    {
        foreach(var item in roomData)
        {
            foreach (var data in item.prefabData)
            {
                data.ResetData();
            }
        }
    }

    public E_RoomTypes GetRandomRoomType()
    {
        return additionalRoomTypes[Random.Range(0, additionalRoomTypes.Length)];
    }

    public Object GetRandomRoomPrefab(E_RoomTypes roomType)
    {
        for (int i = 0; i < roomData.Length; i++)
        {
            if (roomData[i].roomType == roomType)
            {
                return DeterminePrefab(roomData[i].prefabData);
            }

            //TODO check for maximums
        }

        return null;
    }

    Object DeterminePrefab(RoomPrefabData[] prefabData)
    {
        int startIndex = Random.Range(0, prefabData.Length);

        while (true)
        {
            if (prefabData[startIndex].CanUse())
                return prefabData[startIndex].GetRandomPrefab();

            startIndex++;
        }
    }

    public Object GetRandomEnemy()
    {
        return enemies[Random.Range(0, enemies.Length)];
    }

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