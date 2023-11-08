using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public Object wallPiece;
    Vector3 wallLength;
    public Vector3 wallPieceSize;
    public Object dividerPiece;

    private void Start()
    {
        wallLength = transform.localScale;
        transform.localScale = new Vector3(1, 1, 1);
        Cleanup();
        GenerateWalls();
    }

    List<Matrix4x4> wallMatrices;

    void Cleanup()
    {
        int i = 0;
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);

            i++;
            if (i > 1000)
                return;
        }
    }

    void GenerateWalls()
    {
        wallMatrices = new List<Matrix4x4>();

        int wallCount = Mathf.Max(1, (int)(wallLength.x / wallPieceSize.x));
        float scale = (wallLength.x / wallCount) / wallPieceSize.x;

        for(int i = 0; i < wallCount; i++)
        {
            Vector3 pos = transform.position + new Vector3(-wallLength.x / 2 + wallPieceSize.x * scale / 2 + i * scale * wallPieceSize.x, 0, 0);
            Quaternion rot = transform.rotation;
            Vector3 size = new Vector3(scale, 1, 1);

            var mat = Matrix4x4.TRS(pos, rot, size);
            wallMatrices.Add(mat);
        }

        RenderWalls(wallMatrices);
    }

    void RenderWalls(List<Matrix4x4> wallMatrices)
    {
        if (wallMatrices == null) return;

        foreach (var item in wallMatrices)
        {
            GameObject wallGO = Instantiate(wallPiece, this.transform) as GameObject;
            wallGO.transform.position = item.GetPosition();
            wallGO.transform.rotation = item.rotation;

            Vector3 scale = new Vector3();
            scale.x = item.lossyScale.x * wallPieceSize.x;
            scale.y = item.lossyScale.y * wallPieceSize.y;
            scale.z = item.lossyScale.z * wallPieceSize.z;
            wallGO.transform.localScale = scale;
        }

        //Graphics.DrawMeshInstanced(wallPiece, 0, wallMat0, wallMatrices.ToArray(), wallMatrices.Count);
        //Graphics.DrawMeshInstanced(wallPiece, 1, wallMat1, wallMatrices.ToArray(), wallMatrices.Count);
    }
}