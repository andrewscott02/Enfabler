using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enfabler.Quests;

public class DifficultyButton : UpdateGOQuestProgress
{
    public DifficultyMenuManager manager;

    public DifficultyData difficulty;

    public Button button;
    public TextMeshProUGUI buttonText;
    public GameObject unavailableOverlay;

    protected override void Start()
    {
        buttonText.text = difficulty != null ? difficulty.difficultyName : "Select Difficulty";

        base.Start();

        if (quest == null)
        {
            button.interactable = true;
            unavailableOverlay.SetActive(false);
        }
        else
        {
            button.interactable = false;
            unavailableOverlay.SetActive(true);
        }
    }

    public override void QuestUpdated()
    {
        base.QuestUpdated();
        Debug.Log("Quest updated - Unlock interaction");

        button.interactable = true;
        unavailableOverlay.SetActive(false);
    }

    public void ButtonPressed()
    {
        if (difficulty != null)
            manager.ShowDifficulty(difficulty);
        else
            manager.SelectDifficulty();
    }
}