using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableGameObjects_UpdateGOQuestProgress : UpdateGOQuestProgress
{
    public GameObject[] objectsToEnable;
    public GameObject[] objectsToDisable;

    public override void QuestUpdated()
    {
        base.QuestUpdated();
        Debug.Log("Quest updated - Unlock enable/disable objects");
        foreach (var item in objectsToEnable)
        {
            item.SetActive(true);
        }

        foreach (var item in objectsToDisable)
        {
            item.SetActive(false);
        }
    }
}
