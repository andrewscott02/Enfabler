using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CombatTests
{
    #region Parry

    #region Basic Targetting Tests

    [UnityTest]
    public IEnumerator TestParryTargetted1()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        combat.StartBeingAttacked();
        combat.Parry();

        yield return null;

        Assert.AreEqual(true, combat.GetTargetted());
    }

    [UnityTest]
    public IEnumerator TestParryTargetted2()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        combat.StartBeingAttacked();
        combat.StopBeingAttacked();
        combat.Parry();

        yield return null;

        Assert.AreEqual(false, combat.GetTargetted());
    }

    [UnityTest]
    public IEnumerator TestParryTargetted3()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        combat.StartBeingAttacked();
        combat.StartBeingAttacked();
        combat.StopBeingAttacked();
        combat.Parry();

        yield return null;

        Assert.AreEqual(true, combat.GetTargetted());
    }

    #endregion

    #region Tests with model

    [UnityTest]
    public IEnumerator TestParryModelTargetting1()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();

        combat.modelConstructor = model;
        model.test = true;
        model.modelCharacter = gameObject;

        yield return null;

        combat.StartBeingAttacked();
        combat.Parry();

        Assert.AreEqual(Descriptor.Defensive, model.playerState);
    }

    [UnityTest]
    public IEnumerator TestParryModelTargetting2()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();

        combat.modelConstructor = model;
        model.test = true;
        model.modelCharacter = gameObject;

        yield return null;

        combat.StartBeingAttacked();
        combat.StopBeingAttacked();
        combat.Parry();

        Assert.AreEqual(Descriptor.Panic, model.playerState);
    }

    [UnityTest]
    public IEnumerator TestParryModelTargetting3()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();

        combat.modelConstructor = model;
        model.test = true;
        model.modelCharacter = gameObject;

        yield return null;

        combat.StartBeingAttacked();
        combat.StartBeingAttacked();
        combat.StopBeingAttacked();
        combat.Parry();

        Assert.AreEqual(Descriptor.Defensive, model.playerState);
    }

    #endregion

    #endregion

    #region Dodge

    #region Basic Targetting Tests

    [UnityTest]
    public IEnumerator TestDodgeTargetted1()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        combat.StartBeingAttacked();
        combat.Dodge();

        yield return null;

        Assert.AreEqual(true, combat.GetTargetted());
    }

    [UnityTest]
    public IEnumerator TestDodgeTargetted2()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        combat.StartBeingAttacked();
        combat.StopBeingAttacked();
        combat.Dodge();

        yield return null;

        Assert.AreEqual(false, combat.GetTargetted());
    }

    [UnityTest]
    public IEnumerator TestDodgeTargetted3()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        combat.StartBeingAttacked();
        combat.StartBeingAttacked();
        combat.StopBeingAttacked();
        combat.Dodge();

        yield return null;

        Assert.AreEqual(true, combat.GetTargetted());
    }

    #endregion

    #region Tests with model

    [UnityTest]
    public IEnumerator TestDodgeModelTargetting1()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();

        combat.modelConstructor = model;
        model.test = true;
        model.modelCharacter = gameObject;

        yield return null;

        combat.StartBeingAttacked();
        combat.Dodge();

        Assert.AreEqual(Descriptor.Cautious, model.playerState);
    }

    [UnityTest]
    public IEnumerator TestDodgeModelTargetting2()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();

        combat.modelConstructor = model;
        model.test = true;
        model.modelCharacter = gameObject;

        yield return null;

        combat.StartBeingAttacked();
        combat.StopBeingAttacked();
        combat.Dodge();

        Assert.AreEqual(Descriptor.Cautious, model.playerState);
    }

    [UnityTest]
    public IEnumerator TestDodgeModelTargetting3()
    {
        GameObject gameObject = new GameObject();
        CharacterCombat combat = gameObject.AddComponent<CharacterCombat>();
        ConstructPlayerModel model = gameObject.AddComponent<ConstructPlayerModel>();

        combat.modelConstructor = model;
        model.test = true;
        model.modelCharacter = gameObject;

        yield return null;

        combat.StartBeingAttacked();
        combat.StartBeingAttacked();
        combat.StopBeingAttacked();
        combat.Dodge();

        Assert.AreEqual(Descriptor.Cautious, model.playerState);
    }

    #endregion

    #endregion
}
