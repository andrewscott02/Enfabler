using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Quests;

public class ProgressQuest : MonoBehaviour
{
    public Quest[] quests;
    protected int stage = 0;

    protected virtual void ProgressQuestStage()
    {
        if (stage >= quests.Length)
            return;

        if (quests[stage].QuestProgress(true))
            stage++;
    }
}

[System.Serializable]
public enum E_TutorialSteps
{
    OnTriggerEnter, OnTriggerExit, OnHit,
}