using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager instance;

    ArenaFight arenaFight;
    public float interval = 5f;
    public float spawnRadius = 10;

    int round = 0;

    public GameObject fightSelector;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        AIManager.instance.enemiesDied += StartRoundCoroutine;
        PauseMenu.instance.onControlsChange += OnControlsChange;
        fightSelector.SetActive(true);
        StartCoroutine(IShowMouse());
    }

    IEnumerator IShowMouse()
    {
        yield return new WaitForSeconds(0.15f);

        bool usingGamepad = PlayerController.usingGamepad;
        ShowMouse(!usingGamepad);
        if (usingGamepad)
            EventSystem.current.SetSelectedGameObject(fightSelector.transform.GetChild(0).gameObject);
    }

    void ShowMouse(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void SelectFight(ArenaFight fight)
    {
        fightSelector.SetActive(false);
        ShowMouse(false);
        arenaFight = fight;
        StartCoroutine(ISpawnRounds(interval));
    }

    void StartRoundCoroutine()
    {
        if (round >= arenaFight.arenaRounds.Length)
            ArenaWin();
        else
            StartCoroutine(ISpawnRounds(interval));
    }

    IEnumerator ISpawnRounds(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEnemies();
        round++;
    }

    void SpawnEnemies()
    {
        foreach (var item in arenaFight.arenaRounds[round].enemyTypes)
        {
            for (int i = 0; i < item.count; i++)
            {
                Vector3 spawnPos;
                if (!HelperFunctions.GetRandomPointOnNavmesh(transform.position, spawnRadius, 0.5f, 100, out spawnPos))
                {
                    spawnPos = transform.position;
                }

                Instantiate(item.enemyObject, spawnPos, new Quaternion(0, 0, 0, 0));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    public void ArenaWin()
    {
        GameCanvasManager.instance.nextLevelUI.SetActive(true);
        StartCoroutine(IMainMenu(5f));
    }

    public E_Scenes mainMenu;

    IEnumerator IMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(mainMenu.ToString());
    }

    public void OnControlsChange(PlayerInput input)
    {
        bool usingGamepad = input.currentControlScheme == "Gamepad";

        if (fightSelector.activeSelf)
        {
            ShowMouse(!usingGamepad);
        }
        else
        {
            ShowMouse(false);
        }

        if (!usingGamepad) return;

        EventSystem.current.SetSelectedGameObject(fightSelector.transform.GetChild(0).gameObject);
    }
}
