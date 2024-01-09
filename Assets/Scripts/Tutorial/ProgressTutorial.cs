using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressTutorial : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] tutorialMessages;
    protected int stage = 0;

    protected virtual void ProgressTutorialStage()
    {
        if (stage >= tutorialMessages.Length)
            return;

        TutorialManager.instance.SetTutorialMessage(tutorialMessages[stage]);
        stage++;
    }
}

[System.Serializable]
public enum E_TutorialSteps
{
    OnTriggerEnter, OnTriggerExit, OnHit,
}