using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsRoomData", menuName = "PCG/Grammars/RoomData", order = 1)]
public class RoomPrefabData : ScriptableObject
{
    public Object[] roomPrefabs;
    public Vector2Int countMinMax;
    int timesUsed = 0;

    public List<E_Themes> requireThemes = new List<E_Themes>();
    public List<E_Themes> changeThemes = new List<E_Themes>();

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

    public bool CanUse(E_Themes currentTheme, bool change)
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

    public E_Themes GetNextRoomTheme(E_Themes currentThemes)
    {
        int randTheme = Random.Range(0, changeThemes.Count);

        return changeThemes[randTheme];
    }
}

