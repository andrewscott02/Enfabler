using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enfabler.Quests;

public class DungeonButton : UpdateGOQuestProgress
{
    public GrammarsDungeonData dungeonData;
    public int goldCost;

    public DifficultyData forceDifficulty;

    bool questAvailable = false;

    public Button button;
    public TextMeshProUGUI buttonText, goldCostText;
    public GameObject unavailableOverlay, goldOverlay;

    protected override void Start()
    {
        buttonText.text = dungeonData != null ? dungeonData.dungeonName : "Tutorial";

        base.Start();

        if (quest == null)
        {
            questAvailable = true;
        }
        else
        {
            questAvailable = false;
        }

        CheckButtonAvailable();
    }

    private void OnEnable()
    {
        ForceCheckUpdate();
    }

    public override void QuestUpdated()
    {
        base.QuestUpdated();
        Debug.Log("Quest updated - Unlock interaction");

        questAvailable = true;
        CheckButtonAvailable();
    }

    public void CheckButtonAvailable()
    {
        if (questAvailable)
        {
            if (goldCost <= TreasureManager.goldCount)
            {
                button.interactable = true;
                unavailableOverlay.SetActive(false);
            }
            else
            {
                button.interactable = false;
                unavailableOverlay.SetActive(true);
                goldOverlay.SetActive(true);
                goldCostText.text = goldCost.ToString();
            }
        }
        else
        {
            button.interactable = false;
            unavailableOverlay.SetActive(true);
            goldOverlay.SetActive(false);
        }
    }

    public void ButtonPressed()
    {
        if (dungeonData != null)
            DungeonMasterManager.instance.ShowDungeon(dungeonData, forceDifficulty);
        else
            DungeonMasterManager.instance.ShowTutorial();
    }
}