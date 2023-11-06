using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsRoomData", menuName = "PCG/Grammars/RoomData", order = 1)]
public class RoomPrefabData : ScriptableObject
{
    public Object[] roomPrefabs;
    public Vector2Int countMinMax;
    int timesUsed = 0;

    public List<ThemeData> requireThemes;
    public List<ThemeData> changeThemes;

    public void ResetData()
    {
        timesUsed = 0;
    }

    public void Used()
    {
        timesUsed++;
    }

    public Object GetRandomPrefab()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }

    public bool CanUse(ThemeData currentTheme, bool change)
    {
        if (!requireThemes.Contains(currentTheme))
            return false;

        if (change)
        {
            if (changeThemes.Count <= 0)
                return false;
        }
        else
        {
            if (changeThemes.Count > 0)
                return false;
        }

        return (timesUsed < countMinMax.y);
    }

    public ThemeData GetNextRoomTheme(ThemeData currentThemes)
    {
        int randTheme = Random.Range(0, changeThemes.Count);

        return changeThemes[randTheme];
    }
}

