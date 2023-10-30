using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsRoomData", menuName = "PCG/Grammars/RoomData", order = 1)]
public class RoomPrefabData : ScriptableObject
{
    public Object[] roomPrefabs;
    public Vector2Int countMinMax;
    int timesUsed = 0;

    public void ResetData()
    {
        timesUsed = 0;
    }

    public Object GetRandomPrefab()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }

    public bool CanUse()
    {
        return (timesUsed < countMinMax.y);
    }
}
