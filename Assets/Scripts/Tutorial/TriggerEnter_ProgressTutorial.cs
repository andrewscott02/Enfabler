using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnter_ProgressTutorial : ProgressTutorial
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProgressTutorialStage();
        }
    }
}
