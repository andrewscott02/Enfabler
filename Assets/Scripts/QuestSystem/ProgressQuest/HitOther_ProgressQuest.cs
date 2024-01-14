using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

public class HitOther_ProgressQuest : ProgressQuest
{
    public E_DamageEvents[] damageEvents;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CharacterCombat combat = GetComponent<CharacterCombat>();
        combat.onAttackHit += OnHit;
    }

    public void OnHit(E_DamageEvents damageEvent)
    {
        Debug.Log(damageEvent);

        if (stage >= damageEvents.Length)
            return;

        if (damageEvent == damageEvents[stage])
            ProgressQuestStage();
    }
}