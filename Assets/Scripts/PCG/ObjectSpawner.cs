using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public E_ObjectSpawnTypes objectType;
    public bool changeTheme = false;
    public float spawnChance = 1;

    public List<GameObject> SpawnObject(ThemeData theme, GrammarsDungeonData dungeonData, bool trap = false)
    {
        List<GameObject> itemsInRoom = new List<GameObject>();

        if (Random.Range(0f, 1f) > spawnChance)
            return itemsInRoom;

        if (dungeonData.GetRandomObject(this, out int objectIndex, theme, trap))
        {
            GameObject go = Instantiate(theme.objects[objectIndex].objectPrefab, transform) as GameObject;
            itemsInRoom.Add(go);

            foreach (var item in go.GetComponentsInChildren<ObjectSpawner>())
            {
                List<GameObject> rItemsInRoom = item.SpawnObject(theme, dungeonData, trap);

                foreach (var rItem in rItemsInRoom)
                {
                    itemsInRoom.Add(rItem);
                }
            }
        }

        return itemsInRoom;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue - new Color(0, 0, 0, 0.5f);

        Gizmos.DrawSphere(transform.position, 0.5f);

        Gizmos.DrawLine(transform.position, transform.position + (transform.up * 2));
        Gizmos.DrawLine(transform.position + (transform.up * 2), (transform.position + (transform.up * 2)) + (transform.right * 2));
    }
}

[System.Serializable]
public enum E_ObjectSpawnTypes
{
    Corner, Side, Center, SetPiece, WallDecor, WallLight, BesideDoor, Ceiling, Prop, PropSmall
}