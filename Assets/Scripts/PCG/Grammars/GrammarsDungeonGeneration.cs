using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrammarsDungeonGeneration : MonoBehaviour
{
    #region Setup
    
    [System.Serializable]
    public enum E_RoomTypes
    {
        Start, Boss, End,
        Encounter, Puzzle, Treasure
    }

    [System.Serializable]
    public struct RoomData
    {
        public E_RoomTypes roomType;
        public int minimumCount;
    }

    public RoomData[] additionalRoomData;

    #endregion

    public Vector2Int emptyRoomsMinMax;

    [ContextMenu("Generate Grammars Dungeon")]
    public void GenerateGrammarsDungeon()
    {
        List<E_RoomTypes> rooms = new List<E_RoomTypes>() { E_RoomTypes.Start, E_RoomTypes.Boss, E_RoomTypes.End };

        List<E_RoomTypes> additionalRooms = GenerateAdditionalRooms();

        ReplaceDuplicates(additionalRooms);
        EnsureMinimums(additionalRooms);
        
        foreach (var item in additionalRooms)
        {
            rooms.Insert(1, item);
        }

        string dungeonLayout = ConvertToString(rooms);

        Debug.Log(dungeonLayout);
    }

    List<E_RoomTypes> GenerateAdditionalRooms()
    {
        List<E_RoomTypes> rooms = new List<E_RoomTypes>();

        int emptyRoomsCount = Random.Range(emptyRoomsMinMax.x, emptyRoomsMinMax.y);

        for (int i = 0; i < emptyRoomsCount; i++)
        {
            E_RoomTypes roomType = additionalRoomData[Random.Range(0, additionalRoomData.Length)].roomType;
            rooms.Add(roomType);
        }

        return rooms;
    }

    string ConvertToString(List<E_RoomTypes> rooms)
    {
        string dungeonLayout = "";

        foreach (var item in rooms)
            dungeonLayout += item.ToString() + ">";

        dungeonLayout.Remove(dungeonLayout.Length - 1);

        return dungeonLayout;
    }

    #region Rules

    List<E_RoomTypes> ReplaceDuplicates(List<E_RoomTypes> rooms)
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

        for (int i = 0; i  < rooms.Count; i++)
        {
            if (i - 1 >= 0)
            {
                //If this room is the same as the previous room, replace it with a new one
                if (rooms[i].ToString() == rooms[i - 1].ToString())
                {
                    changed = true;
                    rooms[i] = additionalRoomData[Random.Range(0, additionalRoomData.Length)].roomType;
                }
            }
        };

        return rooms;
    }

    List<E_RoomTypes> EnsureMinimums(List<E_RoomTypes> rooms)
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