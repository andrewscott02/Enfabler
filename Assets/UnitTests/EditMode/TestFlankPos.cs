using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestFlankPos
{
    [Test]
    public void TestFlankingPosition1()
    {
        Vector3 actualPos = HelperFunctions.GetFlankingPoint(new Vector3(0, 0, 0), new Vector3(0, 0, 5), 5f);

        Assert.AreEqual(expected: 0, actualPos.x, 0.1f);
        Assert.AreEqual(expected: 0, actualPos.y, 0.1f);
        Assert.AreEqual(expected: 10, actualPos.z, 0.1f);
    }

    [Test]
    public void TestFlankingPosition2()
    {
        Vector3 actualPos = HelperFunctions.GetFlankingPoint(new Vector3(15, 30, 42), new Vector3(6, 12, 16), 1f);

        Assert.AreEqual(expected: 5.7f, actualPos.x, 0.1f);
        Assert.AreEqual(expected: 11.5f, actualPos.y, 0.1f);
        Assert.AreEqual(expected: 15.2f, actualPos.z, 0.1f);
    }

    [Test]
    public void TestFlankingPosition3()
    {
        Vector3 actualPos = HelperFunctions.GetFlankingPoint(new Vector3(15, 75, 81), new Vector3(30, 72, 100), 3f);

        Assert.AreEqual(expected: 31.8f, actualPos.x, 0.1f);
        Assert.AreEqual(expected: 71.6f, actualPos.y, 0.1f);
        Assert.AreEqual(expected: 102.3f, actualPos.z, 0.1f);
    }

    [Test]
    public void TestFlankingPosition4()
    {
        Vector3 actualPos = HelperFunctions.GetFlankingPoint(new Vector3(0, 0, 0), new Vector3(5, 0, -5), -2f);

        Assert.AreEqual(expected: 3.6f, actualPos.x, 0.1f);
        Assert.AreEqual(expected: 0, actualPos.y, 0.1f);
        Assert.AreEqual(expected: -3.6f, actualPos.z, 0.1f);
    }
}
