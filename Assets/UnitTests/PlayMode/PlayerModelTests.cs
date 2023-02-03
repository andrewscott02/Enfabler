using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerModelTests
{
    #region Attack Tests

    [UnityTest]
    public IEnumerator TestSuccessfulAttack()
    {
        GameObject gameObject = new GameObject();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();
        model.test = true;
        
        yield return null;

        model.PlayerAttack(true);

        Assert.AreEqual(Descriptor.Aggressive, model.playerState);
    }

    [UnityTest]
    public IEnumerator TestFailedAttack()
    {
        GameObject gameObject = new GameObject();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();
        model.test = true;

        yield return null;

        model.PlayerAttack(false);

        Assert.AreEqual(Descriptor.Panic, model.playerState);
    }

    [UnityTest]
    public IEnumerator TestCounterAttack()
    {
        GameObject gameObject = new GameObject();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();
        model.test = true;

        yield return null;

        model.PlayerParry(true);
        model.PlayerAttack(true);

        Assert.AreEqual(Descriptor.Counter, model.playerState);
    }

    #endregion

    #region Defensive Tests

    #region Parry Tests

    [UnityTest]
    public IEnumerator TestSuccessfulParry()
    {
        GameObject gameObject = new GameObject();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();
        model.test = true;

        yield return null;

        model.PlayerParry(true);

        Assert.AreEqual(Descriptor.Defensive, model.playerState);
    }

    [UnityTest]
    public IEnumerator TestFailedParry()
    {
        GameObject gameObject = new GameObject();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();
        model.test = true;

        yield return null;

        model.PlayerParry(false);

        Assert.AreEqual(Descriptor.Panic, model.playerState);
    }

    #endregion

    #region Dodge Tests

    [UnityTest]
    public IEnumerator TestSuccessfulDodge()
    {
        GameObject gameObject = new GameObject();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();
        model.test = true;

        yield return null;

        model.PlayerDodge(true);

        Assert.AreEqual(Descriptor.Cautious, model.playerState);
    }

    [UnityTest]
    public IEnumerator TestFailedDodge()
    {
        GameObject gameObject = new GameObject();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();
        model.test = true;

        yield return null;

        model.PlayerDodge(false);

        Assert.AreEqual(Descriptor.Cautious, model.playerState);
    }

    #endregion

    [UnityTest]
    public IEnumerator TestHit()
    {
        GameObject gameObject = new GameObject();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();
        model.test = true;

        yield return null;

        model.PlayerHit();

        Assert.AreEqual(Descriptor.Panic, model.playerState);
    }

    #endregion
}
