using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrammarsThemeData", menuName = "PCG/Grammars/ThemeData", order = 2)]
public class ThemeData : ScriptableObject
{
    public EnemyData[] enemies;
    public ObjectData[] traps, objects;
    public Object[] doors, bosses;
}
