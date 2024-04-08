using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyMenuManager : MonoBehaviour
{
    public static DifficultyMenuManager instance;

    public bool open = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    #region Open/Close Menu

    public GameObject difficultyMenu;

    public List<GameObject> disable;

    public void OpenDifficultyMenu(bool open)
    {
        this.open = open;

        foreach (var item in disable)
        {
            item.SetActive(!open);
        }

        difficultyMenu.SetActive(open);

        if (open && displayDifficulty == null && DifficultyManager.instance != null)
        {
            ShowDifficulty(DifficultyManager.instance.difficulty);
        }
    }

    #endregion

    #region Difficulty Selection

    DifficultyData displayDifficulty;

    public void ShowDifficulty(DifficultyData difficulty)
    {
        displayDifficulty = difficulty;

        UpdateUI();
    }

    public TextMeshProUGUI difficultyTitleText;
    public TextMeshProUGUI difficultyDescText;


    void UpdateUI()
    {
        difficultyTitleText.text = displayDifficulty.difficultyName;
        difficultyDescText.text = displayDifficulty.difficultyDescription;
    }

    public void SelectDifficulty()
    {
        DifficultyManager.instance.difficulty = displayDifficulty;
    }

    #endregion
}