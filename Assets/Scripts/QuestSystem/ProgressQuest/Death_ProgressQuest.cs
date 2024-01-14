using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_ProgressQuest : ProgressQuest
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Health health = GetComponent<Health>();
        health.killDelegate += OnKill;
    }

    public void OnKill(Vector3 attacker, int damage)
    {
        ProgressQuestStage();
    }
}
