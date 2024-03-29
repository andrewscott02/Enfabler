using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using Enfabler.Quests;

public class DungeonMasterManager : MonoBehaviour
{
    public static DungeonMasterManager instance;

    public GameObject dungeonDefaultButton, difficultyDefaultButton;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        OpenDungeonMenu(true);
        OpenDifficultyMenu(true);
        OpenDifficultyMenu(false);
        OpenDungeonMenu(false);
    }

    #region Open/Close Menu

    public GameObject dungeonMasterMenu;

    public List<GameObject> disable;

    public bool TryCloseMenu()
    {
        if (difficultyMenu.open)
        {
            OpenDifficultyMenu(false);
            return false;
        }

        return true;
    }

    public void OpenDungeonMenu(bool open)
    {
        //OpenDifficultyMenu(false);

        foreach (var item in disable)
        {
            item.SetActive(!open);
        }

        dungeonMasterMenu.SetActive(open);
        EventSystem.current.SetSelectedGameObject(dungeonDefaultButton);

        if (open && displayDungeon == null)
        {
            ShowDungeon(initialDungeon);
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
    DifficultyData forceDifficulty;
    int currentDungeonGoldCost = 0;

    public void ShowDungeon(GrammarsDungeonData dungeon, DifficultyData forceDifficulty = null, int goldCost = 0)
    {
        this.forceDifficulty = forceDifficulty;

        displayDungeon = dungeon;
        currentDungeonGoldCost = goldCost;

        CheckDifficultyButton();
        UpdateUI();
    }

    public void ShowTutorial(DifficultyData forceDifficulty)
    {
        this.forceDifficulty = forceDifficulty;
        displayDungeon = null;

        CheckDifficultyButton();
        UpdateUI();
    }

    public TextMeshProUGUI dungeonTitleText;
    public TextMeshProUGUI dungeonDescText;


    void UpdateUI()
    {
        if (displayDungeon == null)
        {
            dungeonTitleText.text = "Tutorial";
            dungeonDescText.text = "Play through the tutorial to learn the basics of playing Enfabler";

            return;
        }

        dungeonTitleText.text = displayDungeon.dungeonName;
        dungeonDescText.text = displayDungeon.dungeonDescription;
    }

    public void Embark()
    {
        if (forceDifficulty != null)
            DifficultyManager.instance.difficulty = forceDifficulty;
        TreasureManager.instance.D_GiveGold(-currentDungeonGoldCost);

        OpenDungeonMenu(false);
        TextPopupManager.instance.ShowMessageText(displayDungeon != null ? "Entering " + displayDungeon.dungeonName : "Entering Tutorial" );
        StartCoroutine(ILoadScene(1.5f));
    }

    public Quest tutorialQuest;

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
            tutorialQuest.ForceRestartQuest();
            SceneManager.LoadScene(E_Scenes.Tutorial.ToString());
        }
    }

    #endregion

    #region DifficultyMenu

    public DifficultyMenuManager difficultyMenu;

    public void OpenDifficultyMenu(bool open)
    {
        EventSystem.current.SetSelectedGameObject(open ? difficultyDefaultButton : dungeonDefaultButton);
        difficultyMenu.OpenDifficultyMenu(open);

        CheckDifficultyButton();
    }

    public TextMeshProUGUI difficultyBtnText;

    public void CheckDifficultyButton()
    {
        string difficultyText = "";
        if (forceDifficulty != null)
            difficultyText = forceDifficulty.difficultyName;
        else if (DifficultyManager.instance != null)
            difficultyText = DifficultyManager.instance.difficulty.difficultyName;

        difficultyBtnText.text = "Current Difficulty: " + difficultyText;
    }

    #endregion
}
