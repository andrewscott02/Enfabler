using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;

    public List<BaseCharacterController> playerTeam;
    public List<BaseCharacterController> enemyTeam;

    private void Start()
    {
        instance = this;
        enemiesDied += EnemiesDied;
        playerDied += PlayerDied;
    }

    public void AllocateTeam(BaseCharacterController character)
    {
        character.characterDied += CharacterDied;

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
            if (playerTeam.Count == 0)
            {
                playerDied();
            }
        }
        else if (enemyTeam.Contains(character))
        {
            enemyTeam.Remove(character);
            if (enemyTeam.Count <= 0)
            {
                enemiesDied();
            }
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

    public GameObject nextLevelUI;

    public void NextLevel()
    {
        nextLevelUI.SetActive(true);
        StartCoroutine(IMainMenu(5f));
    }

    public E_Scenes mainMenu;

    IEnumerator IMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(mainMenu.ToString());
    }

    public delegate void TeamDiedDelegate();
    public TeamDiedDelegate playerDied, enemiesDied;

    public void PlayerDied()
    {
        NextLevel();
    }

    public void EnemiesDied()
    {
        //Debug.Log("Enemies are dead");
        //Empty function for delegate
    }
}
