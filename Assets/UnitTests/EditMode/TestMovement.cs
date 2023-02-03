using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestMovement
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestForward()
    {
        // Use the Assert class to test conditions
        Assert.AreEqual(expected: new Vector2(1, 0), actual: new Vector2(1, 0));
    }
}
