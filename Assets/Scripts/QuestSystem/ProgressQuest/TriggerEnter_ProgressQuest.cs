using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnter_ProgressQuest : ProgressQuest
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProgressQuestStage();
        }
    }
}
