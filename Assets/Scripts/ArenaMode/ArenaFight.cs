using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArenaFight", menuName = "Arena/Fight", order = 0)]
public class ArenaFight : ScriptableObject
{
    public string fightName;
    public ArenaRound[] arenaRounds;

    [System.Serializable]
    public struct ArenaRound
    {
        public ArenaRoundEnemies[] enemyTypes;
    }

    [System.Serializable]
    public struct ArenaRoundEnemies
    {
        public Object enemyObject;
        public int count;
    }
}
