using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIManager : MonoBehaviour
{
    #region Setup

    public static AIManager instance;

    public List<BaseCharacterController> playerTeam;
    public List<BaseCharacterController> enemyTeam;

    private void Awake()
    {
        instance = this;
        enemiesDied += EnemiesDied;
        playerDied += PlayerDied;
    }

    #endregion

    #region Character Deaths

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
            AIController agent = character as AIController;

            if (agent != null)
            {
                enemyActionsQueue.Remove(agent);
                RemoveEnemy(agent);
            }

            enemyTeam.Remove(character);
            if (enemyTeam.Count <= 0)
            {
                enemiesDied();
            }
        }
    }

    public void PlayerDied()
    {
        NextLevel();
    }

    public void EnemiesDied()
    {
        //Debug.Log("Enemies are dead");
        //Empty function for delegate
    }

    #endregion

    #region Teams

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

    #endregion

    #region Scene Management

    public void NextLevel()
    {
        TextPopupManager.instance.ShowMessageText("A hero has fallen...");
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

    #endregion

    #region AI Actions Manager

    public List<AIController> enemyActionsQueue;
    public Vector2 actionsCooldown = new Vector2(2, 5);
    float lastAction = 5;

    public void Enqueue(AIController agent)
    {
        //AddEnemy(agent);
        if (enemyActionsQueue.Contains(agent)) return;

        Debug.Log("Enqueuing agent");
        enemyActionsQueue.Add(agent);
    }

    public void Dequeue(AIController agent, bool attacked)
    {
        if (attacked) lastAction = Random.Range(actionsCooldown.x, actionsCooldown.y);

        if (!enemyActionsQueue.Contains(agent)) return;

        enemyActionsQueue.Remove(agent);
    }

    public bool CanAttack(AIController agent)
    {
        if (agent.ignoreAttackQueue) return true;

        if (lastAction > 0) return false;

        bool canAttack = false;

        if (enemyActionsQueue.Count > 0)
            canAttack = enemyActionsQueue[0] == agent;

        //TODO: Enable attack if enemy is close

        return canAttack;
    }

    private void Update()
    {
        lastAction -= Time.deltaTime;
    }

    #endregion

    #region Combat Mode

    List<AIController> enemiesInCombat = new List<AIController>();

    public int GetEnemiesInCombat()
    {
        return enemiesInCombat.Count;
    }

    public void AddEnemy(AIController enemy)
    {
        if (!enemiesInCombat.Contains(enemy))
        {
            enemiesInCombat.Add(enemy);

            CheckCombatCamera();
        }
    }

    public void RemoveEnemy(AIController enemy)
    {
        if (enemiesInCombat.Contains(enemy))
        {
            enemiesInCombat.Remove(enemy);

            CheckCombatCamera();
        }
    }

    void CheckCombatCamera()
    {
        CameraManager.instance.SetCombatCam(enemiesInCombat.Count > 0);
    }

    #endregion
}
