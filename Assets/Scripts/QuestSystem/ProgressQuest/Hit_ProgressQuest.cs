using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

public class Hit_ProgressQuest : ProgressQuest
{
    public E_AttackType[] specifiedAttacks;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Health health = GetComponent<Health>();
        health.HitReactionDelegate += OnHit;
    }

    public void OnHit(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None)
    {
        //Debug.Log(attackType);

        if (stage >= specifiedAttacks.Length)
            return;

        if (specifiedAttacks[stage] == E_AttackType.None || attackType == specifiedAttacks[stage])
            ProgressQuestStage();
    }
}
