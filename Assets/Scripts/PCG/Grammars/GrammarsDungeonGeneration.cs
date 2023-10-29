using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrammarsDungeonGeneration : MonoBehaviour
{
    #region Setup

    public GrammarsRoomData grammarsRoomData;

    #endregion

    [ContextMenu("Generate Grammars Dungeon")]
    public void GenerateGrammarsDungeon()
    {
        CleanupDungeon();

        List<E_RoomTypes> rooms = new List<E_RoomTypes>() { E_RoomTypes.Start, E_RoomTypes.Healing, E_RoomTypes.Boss, E_RoomTypes.End };

        List<E_RoomTypes> additionalRooms = GenerateAdditionalRooms();

        grammarsRoomData.ReplaceDuplicates(additionalRooms);
        grammarsRoomData.EnsureMinimums(additionalRooms);
        
        foreach (var item in additionalRooms)
        {
            rooms.Insert(1, item);
        }

        string dungeonLayout = ConvertToString(rooms);

        Debug.Log(dungeonLayout);

        GenerateDungeon(rooms);
    }

    [ContextMenu("Cleanup Dungeon")]
    public void CleanupDungeon()
    {
        List<GameObject> children = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < children.Count; i++)
        {
            DestroyImmediate(children[i]);
        }

        createdRooms = new List<PCGRoom>();
    }

    List<E_RoomTypes> GenerateAdditionalRooms()
    {
        List<E_RoomTypes> rooms = new List<E_RoomTypes>();

        int emptyRoomsCount = Random.Range(grammarsRoomData.roomsCountMinMax.x, grammarsRoomData.roomsCountMinMax.y);

        for (int i = 0; i < emptyRoomsCount; i++)
        {
            E_RoomTypes roomType = grammarsRoomData.GetRandomRoom();
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

    List<PCGRoom> createdRooms;

    void GenerateDungeon(List<E_RoomTypes> rooms)
    {
        createdRooms = new List<PCGRoom>();

        for (int i = 0; i < rooms.Count; i++)
        {
            foreach(var data in grammarsRoomData.roomPrefabs)
            {
                if (data.roomType.ToString() == rooms[i].ToString())
                {
                    GameObject go = Instantiate(data.prefab, transform) as GameObject;

                    if (data.roomType != E_RoomTypes.Start)
                    {
                        go.transform.position = createdRooms[i - 1].exitPoint.position;
                        go.transform.rotation = createdRooms[i - 1].exitPoint.rotation;
                    }
                    else
                    {
                        go.transform.position = transform.position;
                        go.transform.rotation = Quaternion.identity;
                    }

                    PCGRoom goRoom = go.GetComponent<PCGRoom>();
                    createdRooms.Add(goRoom);
                }
            };
        }
    }
}