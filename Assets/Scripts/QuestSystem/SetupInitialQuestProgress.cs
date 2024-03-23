using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Quests;

public class SetupInitialQuestProgress : MonoBehaviour
{
    public List<Quest> questsToRestart;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.name + " is resetting quests");
        foreach(var item in questsToRestart)
        {
            item.ForceRestartQuest();
        }
    }
}
