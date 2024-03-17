using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Quests;

public class Dungeon_ProgressQuest : ProgressQuest
{
    public DungeonQuestData[] dungeonQuestData;

    // Start is called before the first frame update
    protected override void Start()
    {
        //Debug.Log("DungeonQuest - Start Function");
        SetupQuests();

        base.Start();
        Interactable interactable = GetComponent<Interactable>();
        interactable.onInteractDelegate += OnInteract;
    }

    public void OnInteract(BaseCharacterController interactCharacter)
    {
        ProgressQuestStage();
    }

    void SetupQuests()
    {
        //Debug.Log("DungeonQuest - Function Called");
        List<Quest> questList = new List<Quest>();

        foreach (var item in dungeonQuestData)
        {
            bool add = true;
            if (item.progressQuest != null)
            {
                if (item.requireDungeonData != null && item.requireDungeonData != DungeonManager.grammarsDungeonData)
                {
                    //Debug.Log("DungeonQuest - Checking Dungeon failed " + DungeonManager.grammarsDungeonData.dungeonName + " for quest " + item.progressQuest.questName + " required data is " + item.requireDungeonData.dungeonName);
                    add = false;
                }
                if (item.requireDifficultyData != null && item.requireDifficultyData != DifficultyManager.instance.difficulty)
                {
                    //Debug.Log("DungeonQuest - Checking Difficulty failed " + DifficultyManager.instance.difficulty + " for quest " + item.progressQuest.questName + " required data is " + item.requireDifficultyData.difficultyName);
                    add = false;
                }
            }
            else
                add = false;

            if (add)
                questList.Add(item.progressQuest);
        }

        quests = new Quest[questList.Count];

        for (int i = 0; i < questList.Count; i++)
        {
            //Debug.Log("DungeonQuest - Adding Quest " + questList[i].questName);
            quests[i] = questList[i];
        }
    }
}

[System.Serializable]
public struct DungeonQuestData
{
    public Quest progressQuest;
    public GrammarsDungeonData requireDungeonData;
    public DifficultyData requireDifficultyData;
}
