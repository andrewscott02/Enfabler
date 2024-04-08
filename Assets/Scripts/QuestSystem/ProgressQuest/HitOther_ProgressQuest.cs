using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

public class HitOther_ProgressQuest : ProgressQuest
{
    public S_DamageEventCollection[] damageEvents;

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

        bool damageEventMatches = false;

        for (int i = 0; i < damageEvents[stage].damageEvents.Length; i++)
        {
            if (damageEvents[stage].damageEvents[i] == damageEvent)
                damageEventMatches = true;
        }

        if (damageEventMatches)
            ProgressQuestStage();
    }
}

[System.Serializable]
public struct S_DamageEventCollection
{
    public E_DamageEvents[] damageEvents;
}