using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockInteraction_UpdateGOQuestProgress : UpdateGOQuestProgress
{
    Interactable interactable;

    protected override void Start()
    {
        base.Start();
        interactable = GetComponent<Interactable>();
    }

    public override void QuestUpdated()
    {
        base.QuestUpdated();
        Debug.Log("Quest updated - Unlock interaction");
        interactable.UnlockInteraction();
    }
}
