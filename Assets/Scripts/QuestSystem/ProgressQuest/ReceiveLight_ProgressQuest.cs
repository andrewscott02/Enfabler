using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveLight_ProgressQuest : ProgressQuest
{
    void Start()
    {
        LightReceiver lightReceiver = GetComponent<LightReceiver>();
        lightReceiver.enableDelegate += OnReceiveLight;
    }

    public void OnReceiveLight()
    {
        ProgressQuestStage();
    }
}