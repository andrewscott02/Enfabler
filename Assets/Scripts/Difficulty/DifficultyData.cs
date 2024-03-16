using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDifficultyData", menuName = "PCG/DifficultyData", order = 0)]
public class DifficultyData : ScriptableObject
{
    public string difficultyName;
    [TextArea(5, 10)]
    public string difficultyDescription;

    #region Rewards

    public float treasureMultiplier;

    #endregion

    #region Enemy Spawn Modifiers

    public float enemySeverityMultiplier;
    public float enemyCountMultiplier;

    #endregion

    #region Enemy Stat Modifiers

    //TODO: Enemy Stat Modifiers

    //public float enemyHealthMultiplier;
    //public float enemyDamageMultiplier;

    #endregion

    //TODO: Available Dungeon Themes
}
