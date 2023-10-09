using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : BaseCharacterController
{
    public override void Start()
    {
        base.Start();
        StartCoroutine(ISequenceAttacks(2f));
    }

    IEnumerator ISequenceAttacks(float interval)
    {
        yield return new WaitForSeconds(interval);
        combat.LightAttack(canCharge: false);
        StartCoroutine(ISequenceAttacks(2f));
    }
}
