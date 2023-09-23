using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;

    public List<BaseCharacterController> playerTeam;
    public List<BaseCharacterController> enemyTeam;

    private void Start()
    {
        instance = this;
    }

    public void AllocateTeam(BaseCharacterController character)
    {
        if (character.playerTeam)
        {
            foreach (var item in playerTeam)
            {
                item.GetCharacterCombat().ignore.Add(character.GetHealth());
            }

            playerTeam.Add(character);
            character.GetCharacterCombat().SetupAllies(playerTeam);
        }
        else
        {
            foreach (var item in enemyTeam)
            {
                item.GetCharacterCombat().ignore.Add(character.GetHealth());
            }

            enemyTeam.Add(character);
            character.GetCharacterCombat().SetupAllies(enemyTeam);
        }
    }

    public void CharacterDied(BaseCharacterController character)
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

        if (playerTeam.Count == 0 || enemyTeam.Count == 0)
        {
            NextLevel();
        }
    }

    public List<BaseCharacterController> GetAllyTeam(BaseCharacterController character)
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

    public List<BaseCharacterController> GetEnemyTeam(BaseCharacterController character)
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

    public bool OnSameTeam(BaseCharacterController a, BaseCharacterController b)
    {
        return GetAllyTeam(a).Contains(b);
    }

    public GameObject nextLevel;

    public void NextLevel()
    {
        nextLevel.SetActive(true);
    }
}
