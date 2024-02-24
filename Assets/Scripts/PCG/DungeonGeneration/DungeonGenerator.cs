using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class DungeonGenerator : MonoBehaviour
{
    #region Setup

    public static DungeonGenerator instance;
    public GrammarsDungeonData grammarsDungeonData;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GenerateDungeon();
        //GameCanvasManager.instance.ShowRegionText(firstTheme.regionName);
    }

    #endregion

    #region Dungeon Generation - Grammars

    public List<E_RoomTypes> rooms;
    public int currentRoom = 0;
    List<PCGRoom> createdRooms;

    [ContextMenu("Generate Dungeon")]
    public void GenerateDungeon()
    {
        instance = this;
        CleanupDungeon();

        int randTheme = Random.Range(0, grammarsDungeonData.startingThemes.Count);

        rooms = new List<E_RoomTypes>() { E_RoomTypes.Start, E_RoomTypes.Healing, E_RoomTypes.Boss, E_RoomTypes.End };

        List<E_RoomTypes> additionalRooms = GenerateAdditionalRooms();
        additionalRooms = GenerateHealingRooms(additionalRooms);
        additionalRooms = DetermineRoomChanges(additionalRooms);

        grammarsDungeonData.ReplaceDuplicates(additionalRooms);
        grammarsDungeonData.EnsureMinimums(additionalRooms);

        foreach (var item in additionalRooms)
        {
            rooms.Insert(1, item);
        }

        string dungeonLayout = ConvertToString(rooms);

        PCGRoom start = GenerateRoom(rooms[0], grammarsDungeonData.startingThemes[randTheme], transform, true, 0);

        BakeNavmesh();
        //PopulateRooms();
        //CullRooms(start);
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

        //createdRooms = new List<PCGRoom>();

        grammarsDungeonData.ResetAllDungeonData();

        currentRoom = 0;
        roomCount = 0;

        rooms = new List<E_RoomTypes>() { E_RoomTypes.Start, E_RoomTypes.Healing, E_RoomTypes.Boss, E_RoomTypes.End };
        createdRooms = new List<PCGRoom>();

        BakeNavmesh();
    }

    #region Grammars

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

    List<E_RoomTypes> DetermineRoomChanges(List<E_RoomTypes> rooms)
    {
        int changeRoomsCount = Random.Range(grammarsDungeonData.themeChanges.y, grammarsDungeonData.themeChanges.y + 1);
        float fInterval = (float)rooms.Count / (float)(changeRoomsCount + 1);
        int interval = Mathf.RoundToInt(fInterval);

        for (int i = 0; i < changeRoomsCount; i++)
        {
            int insertIndex = ((i + 1) * interval);
            //Debug.Log("Insert healing room at " + insertIndex + " from interval " + interval);
            rooms.Insert(insertIndex + i, E_RoomTypes.ChangeTheme);
        }

        return rooms;
    }

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

    string ConvertToString(List<E_RoomTypes> rooms)
    {
        string dungeonLayout = "";

        foreach (var item in rooms)
            dungeonLayout += item.ToString() + ">";

        dungeonLayout.Remove(dungeonLayout.Length - 1);

        return dungeonLayout;
    }

    #endregion

    int roomCount = 0;

    #endregion

    #region Dungeon Generation - Prefabs

    public PCGRoom GenerateRoom(E_RoomTypes roomType, ThemeData theme, Transform spawnTransform, bool mainPath, int removedFromMainPath)
    {
        if (roomCount >= grammarsDungeonData.maxTotalRooms && mainPath == false)
            return null;

        if (removedFromMainPath > grammarsDungeonData.mainPathRemoveLimit)
            return null;

        if (removedFromMainPath == grammarsDungeonData.mainPathRemoveLimit)
        {
            int randInt = Random.Range(0, grammarsDungeonData.sidePathEndRoomTypes.Length);
            roomType = grammarsDungeonData.sidePathEndRoomTypes[randInt];
        }

        if (mainPath)
            currentRoom++;

        roomCount++;

        Object roomPrefab = grammarsDungeonData.GetRandomRoomPrefab(roomType, theme, out ThemeData nextTheme, out bool reversed, out int doorIndex, spawnTransform);

        if (roomPrefab != null)
        {
            GameObject go = Instantiate(roomPrefab, spawnTransform) as GameObject;
            go.transform.SetParent(transform, true);
            PCGRoom goRoom = go.GetComponent<PCGRoom>();
            goRoom.randomDoorPoint = doorIndex;
            goRoom.Setup(roomType, grammarsDungeonData, theme, nextTheme, reversed, mainPath, mainPath ? 0 : removedFromMainPath);

            if (goRoom != null)
            {
                //TODO: Rotate room if reversed
                createdRooms.Add(goRoom);
                return goRoom;
            }
            else
            {
                DestroyImmediate(go);
            }
        }

        return null;
    }

    public bool RoomFits(Object roomPrefab, Transform spawnTransform, out int doorIndex)
    {
        GameObject go = Instantiate(roomPrefab, spawnTransform) as GameObject;
        go.transform.SetParent(transform, true);
        PCGRoom goRoom = go.GetComponent<PCGRoom>();

        doorIndex = Random.Range(0, goRoom.doorPoints.Length);
        goRoom.randomDoorPoint = doorIndex;
        goRoom.GetDoorPoints();

        bool roomFits = !goRoom.Overlaps();
        DestroyImmediate(go);

        return roomFits;
    }

    #endregion

    void PopulateRooms()
    {
        for (int i = 0; i < createdRooms.Count; i++)
        {
            createdRooms[i].PopulateRoom();
        }
    }

    public void CullRooms(PCGRoom currentRoom)
    {
        for (int i = 0; i < createdRooms.Count; i++)
        {
            createdRooms[i].CullRoom(!(createdRooms[i] == currentRoom || currentRoom.attachedRooms.Contains(createdRooms[i])));
        }
    }

    #region Navmesh

    public NavMeshSurface navMeshSurface;

    void BakeNavmesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    #endregion
}