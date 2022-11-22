using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddyAIController : AIController
{
    public override void ActivateAI()
    {
        //Empty
    }

    public override void Update()
    {
        base.Update();
        currentDestination = player.transform.position;
        agent.SetDestination(currentDestination);
    }
}
