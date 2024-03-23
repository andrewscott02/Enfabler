using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Quests;

public class UpdateGOQuestProgress : MonoBehaviour
{
    public Quest quest;
    public E_QuestStates[] states;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (quest != null)
        {
            Debug.Log("Added to quest delegate " + quest.name);
            quest.updateQuestDelegate += CheckUpdate;
        }
    }

    private void OnDestroy()
    {
        if (quest != null)
            quest.updateQuestDelegate -= CheckUpdate;
    }

    protected void ForceCheckUpdate()
    {
        CheckUpdate(quest.state, quest.currentProgress);
    }

    public void CheckUpdate(E_QuestStates questState, int progress)
    {
        Debug.Log("Quest updated - Received message");

        foreach (var item in states)
        {
            if (item == questState)
            {
                Debug.Log("Quest updated - Quest state is correct");
                QuestUpdated();
                return;
            }
        }
    }

    public virtual void QuestUpdated()
    {
        //Override in other scripts
    }
}
