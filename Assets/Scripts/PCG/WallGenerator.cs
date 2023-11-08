using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public bool changeTheme = false;
    ThemeData theme;

    public GameObject cube;

    Object wallPiece;
    Vector3 wallLength;
    Vector3 wallPieceSize = new Vector3(4, 2.9f, 1);

    Quaternion originalRot;

    public void SetupRoom(ThemeData theme)
    {
        if (gameObject.CompareTag("Wall"))
        {
            wallPiece = theme.wallPiece;
            wallPieceSize = theme.wallPieceSize;
        }
        else if (gameObject.CompareTag("Ceiling"))
        {
            wallPiece = theme.ceilingPiece;
            wallPieceSize = theme.ceilingPieceSize;
        }
        else if (gameObject.CompareTag("Floor"))
        {
            wallPiece = theme.floorPiece;
            wallPieceSize = theme.floorPieceSize;
        }

        this.theme = theme;
        wallLength = new Vector3();
        wallLength.x = transform.localScale.x;
        wallLength.y = transform.localScale.y;
        transform.localScale = new Vector3(1, 1, 1);

        originalRot = transform.rotation;

        transform.rotation = Quaternion.identity;

        Cleanup();
        GenerateWalls();

        transform.rotation = originalRot;
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

        Vector2Int wallCount = new Vector2Int();
        wallCount.x = Mathf.Max(1, (int)(wallLength.x / wallPieceSize.x));
        wallCount.y = Mathf.Max(1, (int)(wallLength.y / wallPieceSize.y));

        Vector2 scale = new Vector2();
        scale.x = (wallLength.x / wallCount.x) / wallPieceSize.x;
        scale.y = (wallLength.y / wallCount.y) / wallPieceSize.y;

        for(int y = 0; y < wallCount.y; y++)
        {
            for (int x = 0; x < wallCount.x; x++)
            {
                Vector3 pos = transform.position + 
                    new Vector3(
                        -wallLength.x / 2 + wallPieceSize.x * scale.x / 2 + x * scale.x * wallPieceSize.x,
                        (wallPieceSize.y * scale.y) * y, 
                        0);
                
                Quaternion rot = transform.rotation;
                Vector3 size = new Vector3(scale.x, scale.y, 1);

                var mat = Matrix4x4.TRS(pos, rot, size);
                wallMatrices.Add(mat);
            }
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
            wallGO.transform.localScale = item.lossyScale;
        }

        //Graphics.DrawMeshInstanced(wallPiece, 0, wallMat0, wallMatrices.ToArray(), wallMatrices.Count);
        //Graphics.DrawMeshInstanced(wallPiece, 1, wallMat1, wallMatrices.ToArray(), wallMatrices.Count);
    }
}