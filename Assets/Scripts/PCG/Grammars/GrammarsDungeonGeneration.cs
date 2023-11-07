using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class GrammarsDungeonGeneration : MonoBehaviour
{
    #region Setup

    public GrammarsDungeonData grammarsDungeonData;

    #endregion

    private void Start()
    {
        GenerateGrammarsDungeon();
    }

    [ContextMenu("Generate Grammars Dungeon")]
    public void GenerateGrammarsDungeon()
    {
        CleanupDungeon();

        int randTheme = Random.Range(0, grammarsDungeonData.startingThemes.Count);
        currentTheme = grammarsDungeonData.startingThemes[randTheme];

        List<E_RoomTypes> rooms = new List<E_RoomTypes>() { E_RoomTypes.Start, E_RoomTypes.Healing, E_RoomTypes.Boss, E_RoomTypes.End };

        List<E_RoomTypes> additionalRooms = GenerateAdditionalRooms();
        additionalRooms = GenerateHealingRooms(additionalRooms);

        grammarsDungeonData.ReplaceDuplicates(additionalRooms);
        grammarsDungeonData.EnsureMinimums(additionalRooms);
        
        foreach (var item in additionalRooms)
        {
            rooms.Insert(1, item);
        }

        string dungeonLayout = ConvertToString(rooms);

        Debug.Log(dungeonLayout);

        GenerateDungeonRooms(rooms);
        BakeNavmesh();

        PopulateRooms();
    }

    [ContextMenu("Cleanup Dungeon")]
    public void CleanupDungeon()
    {
        PCGRoom[] roomData = GetComponentsInChildren<PCGRoom>();

        foreach (var item in roomData)
        {
            item.DeleteRoom();
        }

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

        grammarsDungeonData.ResetAllDungeonData();
    }

    #region Creating Rooms

    ThemeData currentTheme;

    List<E_RoomTypes> GenerateAdditionalRooms()
    {
        List<E_RoomTypes> rooms = new List<E_RoomTypes>();

        int emptyRoomsCount = Random.Range(grammarsDungeonData.roomsCountMinMax.x, grammarsDungeonData.roomsCountMinMax.y + 1);

        for (int i = 0; i < emptyRoomsCount; i++)
        {
            E_RoomTypes roomType = grammarsDungeonData.GetRandomRoomType();
            rooms.Add(roomType);
        }

        return rooms;
    }

    List<E_RoomTypes> GenerateHealingRooms(List<E_RoomTypes> rooms)
    {
        int healingRoomsCount = grammarsDungeonData.additionalHealingRooms;
        float fInterval = (float)rooms.Count / (float)(healingRoomsCount + 1);
        int interval = Mathf.RoundToInt(fInterval);

        for (int i = 0; i < healingRoomsCount; i++)
        {
            int insertIndex = ((i + 1) * interval);
            //Debug.Log("Insert healing room at " + insertIndex + " from interval " + interval);
            rooms.Insert(insertIndex + i, E_RoomTypes.Healing);
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

    List<Object> DetermineDungeonRooms(List<E_RoomTypes> rooms, out List<ThemeData> themes)
    {
        List<Object> prefabs = new List<Object>();
        themes = new List<ThemeData>();

        List<int> roomChanges = DetermineRoomChanges(rooms);

        for(int i = 0; i < rooms.Count; i++)
        {
            bool change = roomChanges.Contains(i);
            Object prefab = grammarsDungeonData.GetRandomRoomPrefab(rooms[i], currentTheme, change, out ThemeData nextRoom);
            if (prefab != null)
            {
                prefabs.Add(prefab);
                themes.Add(currentTheme);
            }
            currentTheme = nextRoom;
        }

        //TODO: Use grammars to change rooms

        return prefabs;
    }

    List<int> DetermineRoomChanges(List<E_RoomTypes> rooms)
    {
        //Get rooms at aregular interval, but do not add healing rooms
        int changeRoomsCount = Random.Range(grammarsDungeonData.themeChanges.y, grammarsDungeonData.themeChanges.y + 1);
        List<int> changeRooms = new List<int>();

        float fInterval = (float)rooms.Count / (float)(changeRoomsCount + 1);
        int interval = Mathf.RoundToInt(fInterval);

        for (int i = 0; i < changeRoomsCount; i++)
        {
            int roomIndex = ((i + 1) * interval);
            if (rooms[roomIndex] == E_RoomTypes.Healing)
            {
                changeRooms.Add(roomIndex + 1);
            }
            else
            {
                changeRooms.Add(roomIndex);
            }
        }

        string debug = "change rooms at ";
        foreach(var item in changeRooms)
        {
            debug += item.ToString() + ", ";
        }

        Debug.Log(debug);

        return changeRooms;
    }

    void GenerateDungeonRooms(List<E_RoomTypes> rooms)
    {
        createdRooms = new List<PCGRoom>();

        List<Object> prefabs = DetermineDungeonRooms(rooms, out List<ThemeData> themes);

        for (int i = 0; i < rooms.Count; i++)
        {
            foreach(var data in grammarsDungeonData.roomData)
            {
                if (data.roomType.ToString() == rooms[i].ToString())
                {
                    GameObject go = Instantiate(prefabs[i], transform) as GameObject;

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
                    goRoom.Setup(rooms[i], grammarsDungeonData, themes[i]);
                    createdRooms.Add(goRoom);
                }
            };
        }
    }

    void PopulateRooms()
    {
        for (int i = 0; i < createdRooms.Count; i++)
        {
            createdRooms[i].PopulateRoom();
        }
    }

    #endregion

    public NavMeshSurface navMeshSurface;

    void BakeNavmesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}