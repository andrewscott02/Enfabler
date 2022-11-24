using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddyAIController : AIController
{
    public override void BehaviourTree()
    {
        base.BehaviourTree();

        if (currentTarget == null || player != null)
        {
            currentDestination = player.transform.position;
            agent.SetDestination(currentDestination);
        }
    }
}
