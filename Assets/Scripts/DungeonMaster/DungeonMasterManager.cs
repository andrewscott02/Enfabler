using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DungeonMasterManager : MonoBehaviour
{
    public static DungeonMasterManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    #region Open/Close Menu

    public GameObject dungeonMasterMenu;

    public List<GameObject> disable;

    public void OpenDungeonMenu(bool open)
    {
        foreach (var item in disable)
        {
            item.SetActive(!open);
        }

        dungeonMasterMenu.SetActive(open);

        if (open && displayDungeon == null)
        {
            ShowDungeon(initialDungeon);
            OpenDifficultyMenu(false);
        }
    }

    public void CloseMenu()
    {
        PauseMenu.instance.ShowDungeonMasterMenu(false);
    }

    #endregion

    #region Dungeon Selection

    GrammarsDungeonData displayDungeon;
    public GrammarsDungeonData initialDungeon;

    public void ShowDungeon(GrammarsDungeonData dungeon)
    {
        displayDungeon = dungeon;

        UpdateUI();
    }

    public void ShowTutorial()
    {
        displayDungeon = null;

        UpdateUI();
    }

    public TextMeshProUGUI difficultyTitleText;
    public TextMeshProUGUI difficultyDescText;


    void UpdateUI()
    {
        if (displayDungeon == null)
        {
            difficultyTitleText.text = "Tutorial";
            difficultyDescText.text = "Play through the tutorial to learn the basics of playing Enfabler";

            return;
        }

        difficultyTitleText.text = displayDungeon.dungeonName;
        difficultyDescText.text = displayDungeon.dungeonDescription;
    }

    public void Embark()
    {
        //TODO Load dungeon with selected difficulty
        OpenDungeonMenu(false);
        TextPopupManager.instance.ShowMessageText(displayDungeon != null ? "Entering " + displayDungeon.dungeonName : "Entering Tutorial" );
        StartCoroutine(ILoadScene(1.5f));
    }

    IEnumerator ILoadScene(float delay)
    {
        LoadingScreen.instance.StartLoadingScreen();

        yield return new WaitForSeconds(delay);

        if (displayDungeon != null)
        {
            DungeonManager.grammarsDungeonData = displayDungeon;
            SceneManager.LoadScene(E_Scenes.PCGGrammars.ToString());
        }
        else
        {
            SceneManager.LoadScene(E_Scenes.Tutorial.ToString());
        }
    }

    #endregion

    #region DifficultyMenu

    public DifficultyMenuManager difficultyMenu;

    public void OpenDifficultyMenu(bool open)
    {
        difficultyMenu.OpenDifficultyMenu(open);

        CheckDifficultyButton();
    }

    public TextMeshProUGUI difficultyBtnText;

    public void CheckDifficultyButton()
    {
        difficultyBtnText.text = "Current Difficulty: " + DifficultyManager.instance.difficulty.difficultyName;
    }

    #endregion
}
