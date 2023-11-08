using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsThemeData", menuName = "PCG/Grammars/ThemeData", order = 2)]
public class ThemeData : ScriptableObject
{
    [Header("Room Generation Data")]
    public Object wallPiece;
    public Vector3 wallPieceSize = new Vector3(4, 2.9f, 1);
    public Object ceilingPiece;
    public Vector3 ceilingPieceSize = new Vector3(4, 2.9f, 1);
    public Object floorPiece;
    public Vector3 floorPieceSize = new Vector3(4, 2.9f, 1);
    public Object pillarPiece;
    public Vector3 pillarPieceSize = new Vector3(4, 2.9f, 1);

    [Header("Populate Room Data")]
    public EnemyData[] enemies;
    public ObjectData[] traps, objects;
    public Object[] doors, bosses;
}
