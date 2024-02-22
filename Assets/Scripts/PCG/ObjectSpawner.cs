using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public E_ObjectSpawnTypes objectType;

    public BoundingBoxData boundingBoxData = new BoundingBoxData
    {
        offset = new Vector3(0, 0, 0),
        size = new Vector3(0, 0, 0)
    };

    public bool changeTheme = false;
    public float spawnChance = 1;

    public ObjectSpawner linkedSpawner;
    public int spawnedObjectIndex { get; private set; } = -1;

    public List<GameObject> SpawnObject(ThemeData theme, GrammarsDungeonData dungeonData, bool trap = false)
    {
        List<GameObject> itemsInRoom = new List<GameObject>();

        if (transform.childCount > 0)
            return itemsInRoom;

        if (Random.Range(0f, 1f) > spawnChance)
            return itemsInRoom;

        int objectIndex = -1;

        if (linkedSpawner != null)
        {
            if (linkedSpawner.spawnedObjectIndex >= 0)
            {
                if (theme.objects[linkedSpawner.spawnedObjectIndex].canLink)
                    objectIndex = linkedSpawner.spawnedObjectIndex;
            }
        }

        if (objectIndex < 0)
        {
            if (!dungeonData.GetRandomObject(this, out objectIndex, theme, trap))
                return itemsInRoom;
        }

        GameObject go = Instantiate(theme.objects[objectIndex].objectPrefab, transform) as GameObject;
        go.transform.position = GetSpawnPosition(theme.objects[objectIndex]);
        go.transform.localRotation = Quaternion.Euler(GetSpawnRotation(theme.objects[objectIndex]));
        itemsInRoom.Add(go);

        foreach (var item in go.GetComponentsInChildren<ObjectSpawner>())
        {
            List<GameObject> rItemsInRoom = item.SpawnObject(theme, dungeonData, trap);

            foreach (var rItem in rItemsInRoom)
            {
                itemsInRoom.Add(rItem);
            }
        }

        //If spawned object links, call spawn specified object on linked spawner if it is not already the same

        return itemsInRoom;
    }

    public void SpawnSpecifiedObject(int objectIndex)
    {
        //delete children in spawner and spawn specified object
    }

    Vector3 GetSpawnPosition(ObjectData spawnObject)
    {
        Vector3 spawnPos = transform.position;

        if (spawnObject.randomPositiont > 0)
        {
            spawnPos += boundingBoxData.offset;

            spawnPos.x += Random.Range(-boundingBoxData.size.x / 2, boundingBoxData.size.x / 2);
            spawnPos.y += Random.Range(-boundingBoxData.size.y / 2, boundingBoxData.size.y / 2);
            spawnPos.z += Random.Range(-boundingBoxData.size.z / 2, boundingBoxData.size.z / 2);

            spawnPos = RotatePointAroundPivot(spawnPos, transform.position, transform.eulerAngles);

            spawnPos = HelperFunctions.LerpVector3(transform.position, spawnPos, spawnObject.randomPositiont);
        }

        return spawnPos;
    }

    Vector3 GetSpawnRotation(ObjectData spawnObject)
    {
        Vector3 spawnRot = new Vector3();

        spawnRot.x += Random.Range(-spawnObject.randomRotationAxes.x, spawnObject.randomRotationAxes.x);
        spawnRot.y += Random.Range(-spawnObject.randomRotationAxes.y, spawnObject.randomRotationAxes.y);
        spawnRot.z += Random.Range(-spawnObject.randomRotationAxes.z, spawnObject.randomRotationAxes.z);
        //spawnRot.w += Random.Range(-spawnObject.randomRotationAxes.w, spawnObject.randomRotationAxes.w);

        return spawnRot;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = transform.localToWorldMatrix;

        if (objectType == E_ObjectSpawnTypes.Door)
        {
            Gizmos.color = (changeTheme ? Color.red : Color.green) - new Color(0, 0, 0, 0.5f);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawCube(transform.up * 2.3f, new Vector3(4.75f, 4.6f, 1));
            return;
        }

        Gizmos.color = (changeTheme ? Color.red : Color.blue) - new Color(0, 0, 0, 0.5f);

        Gizmos.DrawSphere(transform.position, 0.5f);

        Gizmos.DrawLine(transform.position, transform.position + (transform.up * 2));
        Gizmos.DrawLine(transform.position + (transform.up * 2), (transform.position + (transform.up * 2)) + (transform.right * 2));

        Gizmos.color -= new Color(0, 0, 0, 0.2f);

        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(boundingBoxData.offset, boundingBoxData.size);
    }

    #region Test Spawning

    public ThemeData testTheme;
    public GrammarsDungeonData testDungeonData;

    [ContextMenu("Spawn Test Object")]
    public void Test()
    {
        if (transform.childCount == 1)
            DestroyImmediate(transform.GetChild(0).gameObject);
        SpawnObject(testTheme, testDungeonData, false);
        testDungeonData.ResetObjectData();
    }

    #endregion
}

[System.Serializable]
public enum E_ObjectSpawnTypes
{
    Corner, Side, Center, SetPiece, WallDecor, WallLight, BesideDoor, CeilingLight, CeilingDecor, Prop, PropSmall, Door
}

[System.Serializable]
public struct BoundingBoxData
{
    public Vector3 offset;
    public Vector3 size;
}