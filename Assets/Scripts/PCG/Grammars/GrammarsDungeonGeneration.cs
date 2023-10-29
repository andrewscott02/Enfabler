using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrammarsDungeonGeneration : MonoBehaviour
{
    enum E_RoomTypes
    {
        Start, Boss, End,
        Encounter, Puzzle, Treasure
    }

    public int emptyRooms;

    List<E_RoomTypes> additionalRoomTypes = new List<E_RoomTypes>()
    { 
        E_RoomTypes.Encounter, E_RoomTypes.Puzzle, E_RoomTypes.Treasure
    };

    [ContextMenu("Generate Grammars Dungeon")]
    public void GenerateGrammarsDungeon()
    {
        List<E_RoomTypes> rooms = GenerateGrammars();

        string dungeonLayout = "";

        foreach (var item in rooms)
            dungeonLayout += item.ToString() + ">";

        dungeonLayout.Remove(dungeonLayout.Length - 1);

        Debug.Log(dungeonLayout);
    }

    List<E_RoomTypes> GenerateGrammars()
    {
        List<E_RoomTypes> rooms = new List<E_RoomTypes>() { E_RoomTypes.Start, E_RoomTypes.Boss, E_RoomTypes.End };

        for (int i = 0; i < emptyRooms; i++)
        {
            rooms.Insert(1, additionalRoomTypes[Random.Range(0, additionalRoomTypes.Count)]);
        }

        return rooms;
    }
}
