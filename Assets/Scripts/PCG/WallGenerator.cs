using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public ThemeData theme;

    Vector3 wallLength;
    public Object dividerPiece;

    private void Start()
    {
        wallLength = new Vector3();
        wallLength.x = transform.localScale.x;
        wallLength.y = transform.localScale.y;
        transform.localScale = new Vector3(1, 1, 1);
        //Cleanup();
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

        Vector2Int wallCount = new Vector2Int();
        wallCount.x = Mathf.Max(1, (int)(wallLength.x / theme.wallPieceSize.x));
        wallCount.y = Mathf.Max(1, (int)(wallLength.y / theme.wallPieceSize.y));

        Vector2 scale = new Vector2();
        scale.x = (wallLength.x / wallCount.x) / theme.wallPieceSize.x;
        scale.y = (wallLength.y / wallCount.y) / theme.wallPieceSize.y;

        for(int y = 0; y < wallCount.y; y++)
        {
            for (int x = 0; x < wallCount.x; x++)
            {
                Vector3 pos = transform.position + 
                    new Vector3(
                        -wallLength.x / 2 + theme.wallPieceSize.x * scale.x / 2 + x * scale.x * theme.wallPieceSize.x,
                        (-wallLength.y / 2 + theme.wallPieceSize.y * scale.y / 2 + y * scale.y * theme.wallPieceSize.y) + ((scale.y * theme.wallPieceSize.y) / 2), 
                        0);
                
                Quaternion rot = transform.rotation;
                Vector3 size = new Vector3(scale.x, scale.y, scale.y);

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
            GameObject wallGO = Instantiate(theme.wallPiece, this.transform) as GameObject;
            wallGO.transform.position = item.GetPosition();
            wallGO.transform.rotation = item.rotation;
            wallGO.transform.localScale = item.lossyScale;
        }

        //Graphics.DrawMeshInstanced(wallPiece, 0, wallMat0, wallMatrices.ToArray(), wallMatrices.Count);
        //Graphics.DrawMeshInstanced(wallPiece, 1, wallMat1, wallMatrices.ToArray(), wallMatrices.Count);
    }
}