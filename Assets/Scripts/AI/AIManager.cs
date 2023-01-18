using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;

    public List<CharacterController> playerTeam;
    public List<CharacterController> enemyTeam;

    private void Start()
    {
        instance = this;
    }

    public void AllocateTeam(CharacterController character)
    {
        if (character.playerTeam)
        {
            playerTeam.Add(character);
        }
        else
        {
            enemyTeam.Add(character);
        }
    }

    public void CharacterDied(CharacterController character)
    {
        if (playerTeam.Contains(character))
        {
            playerTeam.Remove(character);
        }
        else if (enemyTeam.Contains(character))
        {
            enemyTeam.Remove(character);
        }

        Destroy(character.gameObject);
    }

    public List<CharacterController> GetAllyTeam(CharacterController character)
    {
        if (character.playerTeam)
        {
            return playerTeam;
        }
        else
        {
            return enemyTeam;
        }
    }

    public List<CharacterController> GetEnemyTeam(CharacterController character)
    {
        if (character.playerTeam)
        {
            return enemyTeam;
        }
        else
        {
            return playerTeam;
        }
    }

    public bool OnSameTeam(CharacterController a, CharacterController b)
    {
        return GetAllyTeam(a).Contains(b);
    }
}
