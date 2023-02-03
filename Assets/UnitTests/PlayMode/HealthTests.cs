using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HealthTests
{
    #region Damage Tests

    [UnityTest]
    public IEnumerator TestDamage()
    {
        GameObject gameObject = new GameObject();
        Health health = gameObject.AddComponent<Health>();
        
        yield return null;

        Vector3 nullVector = new Vector3(0, 0, 0);
        health.Damage(null, 15, nullVector, nullVector);

        Assert.AreEqual(health.maxHealth - 15, health.GetCurrentHealth());
    }

    [UnityTest]
    public IEnumerator TestKill()
    {
        GameObject gameObject = new GameObject();
        Health health = gameObject.AddComponent<Health>();

        yield return null;

        Vector3 nullVector = new Vector3(0, 0, 0);
        health.Damage(null, 50, nullVector, nullVector);

        Assert.AreEqual(health.maxHealth - 50, health.GetCurrentHealth());
        Assert.AreEqual(true, health.dying);
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

        Assert.AreEqual(health.maxHealth, health.GetCurrentHealth());
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

        Assert.AreEqual(health.maxHealth, health.GetCurrentHealth());
    }

    #endregion

    #region Heal Tests

    [UnityTest]
    public IEnumerator TestHealingMaxHealth()
    {
        GameObject gameObject = new GameObject();
        Health health = gameObject.AddComponent<Health>();

        yield return null;

        Vector3 nullVector = new Vector3(0, 0, 0);
        health.Heal(15);

        Assert.AreEqual(health.maxHealth, health.GetCurrentHealth());
    }

    [UnityTest]
    public IEnumerator TestHealing1()
    {
        GameObject gameObject = new GameObject();
        Health health = gameObject.AddComponent<Health>();

        yield return null;

        Vector3 nullVector = new Vector3(0, 0, 0);
        health.Damage(null, 25, nullVector, nullVector);
        health.Heal(10);

        Assert.AreEqual(health.maxHealth - 15, health.GetCurrentHealth());
    }

    [UnityTest]
    public IEnumerator TestHealing2()
    {
        GameObject gameObject = new GameObject();
        Health health = gameObject.AddComponent<Health>();

        yield return null;

        Vector3 nullVector = new Vector3(0, 0, 0);
        health.Damage(null, 15, nullVector, nullVector);
        health.Heal(20);

        Assert.AreEqual(health.maxHealth, health.GetCurrentHealth());
    }

    #endregion
}
