using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementTest
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestForwardMovement()
    {
        GameObject gameObject = new GameObject();
        PlayerMovement controller = gameObject.AddComponent<PlayerMovement>();
        
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return new WaitForSeconds(0.5f);

        controller.SetMovement(1, 0);

        Vector3 expectedMovement = new Vector3(0, 0, 1) * Time.deltaTime;
        expectedMovement = controller.transform.TransformDirection(expectedMovement);

        Assert.AreEqual(expectedMovement.z, controller.movement.z, 0.1f);
    }
}
