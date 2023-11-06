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

    public ThemeData GetNextRoomTheme(ThemeData currentTheme)
    {
        int randThemeStart = Random.Range(0, changeThemes.Count);
        int randTheme = randThemeStart;

        while (true)
        {
            if (changeThemes[randTheme] != currentTheme)
            {
                return changeThemes[randTheme];
            }

            randTheme++;

            if (randTheme >= changeThemes.Count)
                randTheme = 0;

            if (randTheme == randThemeStart)
                return changeThemes[randTheme];
        }
    }
}

