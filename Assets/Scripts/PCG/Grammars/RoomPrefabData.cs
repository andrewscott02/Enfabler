using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsRoomData", menuName = "PCG/Grammars/RoomData", order = 1)]
public class RoomPrefabData : ScriptableObject
{
    public bool reverseRooms = false;
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

    public Object GetRandomPrefab(Transform spawnTransform, out int doorIndex)
    {
        int startIndex = Random.Range(0, roomPrefabs.Length);
        int currentIndex = startIndex;

        while (true)
        {
            if (DungeonGenerator.instance.RoomFits(roomPrefabs[currentIndex], spawnTransform, reverseRooms, out doorIndex))
            {
                return roomPrefabs[currentIndex];
            }

            currentIndex++;

            if (currentIndex >= roomPrefabs.Length)
                currentIndex = 0;

            if (currentIndex == startIndex)
                return null;
        }
    }

    public bool CanUse(ThemeData currentTheme)
    {
        if (!requireThemes.Contains(currentTheme))
            return false;

        if (changeThemes != null && changeThemes.Count > 0)
        {
            bool nextThemesAvailable = false;

            foreach (var item in changeThemes)
            {
                //Debug.Log("ChangeThemes: Checking theme " + item + " for room " + name);
                if (DifficultyContainsTheme(item))
                {
                    //Debug.LogWarning("ChangeThemes: Warning, theme is available in " + item.regionName + " in " + name);
                    nextThemesAvailable = true;
                    break;
                }
            }

            if (!nextThemesAvailable)
            {
                Debug.LogWarning("Warning, no themes are available in room " + currentTheme + ": " + name);
                return false;
            }
        }

        return (timesUsed < countMinMax.y);
    }

    public ThemeData GetNextRoomTheme(ThemeData currentTheme)
    {
        if (changeThemes.Count == 0) return currentTheme;

        int randThemeStart = Random.Range(0, changeThemes.Count);
        int randTheme = randThemeStart;

        while (true)
        {
            if (changeThemes[randTheme] != currentTheme && DifficultyContainsTheme(changeThemes[randTheme]))
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

    public bool DifficultyContainsTheme(ThemeData theme)
    {
        if (DifficultyManager.instance == null)
            return true;

        return DifficultyManager.instance.difficulty.allThemes.Contains(theme);
    }
}

