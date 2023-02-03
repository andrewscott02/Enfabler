using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HealthTests
{
    [UnityTest]
    public IEnumerator TestDamage()
    {
        GameObject gameObject = new GameObject();
        Health health = gameObject.AddComponent<Health>();
        
        yield return null;

        Vector3 nullVector = new Vector3(0, 0, 0);
        health.Damage(null, 15, nullVector, nullVector);

        Assert.AreEqual(35, health.GetCurrentHealth());
    }

    [UnityTest]
    public IEnumerator TestParry()
    {
        GameObject gameObject = new GameObject();
        Health health = gameObject.AddComponent<Health>();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        combat.StartParry();

        yield return null;

        Vector3 nullVector = new Vector3(0, 0, 0);
        health.Damage(null, 15, nullVector, nullVector);

        Assert.AreEqual(50, health.GetCurrentHealth());
    }

    [UnityTest]
    public IEnumerator TestDodge()
    {
        GameObject gameObject = new GameObject();
        Health health = gameObject.AddComponent<Health>();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        combat.StartDodge();

        yield return null;

        Vector3 nullVector = new Vector3(0, 0, 0);
        health.Damage(null, 15, nullVector, nullVector);

        Assert.AreEqual(50, health.GetCurrentHealth());
    }
}
