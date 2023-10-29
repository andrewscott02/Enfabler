using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsRoomData", menuName = "PCG/RoomData/GrammarsRoomData", order = 0)]
public class GrammarsRoomData : ScriptableObject
{
    public RoomPrefabData[] roomPrefabs;
    public RoomData[] additionalRoomData;
    public Vector2Int roomsCountMinMax;

    public E_RoomTypes GetRandomRoom()
    {
        return additionalRoomData[Random.Range(0, additionalRoomData.Length)].roomType;
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
                    rooms[i] = GetRandomRoom();
                }
            }
        };

        return rooms;
    }

    public List<E_RoomTypes> EnsureMinimums(List<E_RoomTypes> rooms)
    {
        for (int i = 0; i < additionalRoomData.Length; i++)
        {
            int count = 0;

            foreach (var item in rooms)
            {
                if (item.ToString() == additionalRoomData[i].roomType.ToString())
                    count++;
            }

            if (count < additionalRoomData[i].minimumCount)
                rooms.Add(additionalRoomData[i].roomType);
        }

        return rooms;
    }

    #endregion
}

[System.Serializable]
public struct RoomData
{
    public E_RoomTypes roomType;
    public int minimumCount;
}

[System.Serializable]
public struct RoomPrefabData
{
    public E_RoomTypes roomType;
    public Object[] prefabs;
}

[System.Serializable]
public enum E_RoomTypes
{
    Start, Boss, End,
    Encounter, Puzzle, Treasure, Healing
}

