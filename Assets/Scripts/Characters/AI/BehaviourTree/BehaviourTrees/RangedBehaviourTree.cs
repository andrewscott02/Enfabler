using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class RangedBehaviourTree : BehaviourTree
{
    protected override Node SetupTree()
    {
        //Debug.Log("Setting up basic BT for " + agent.name);

        Node root = new Selector(

            BaseBehaviours.CastSpell(agent),

            BaseBehaviours.AttackClosestTarget(agent, true),

            //Checks if the closest enemy is within sight range and moves towards it if true
            BaseBehaviours.MoveToClosestTarget(agent, agent.distanceAllowance, true),
            //If there are no targets, but the player is an ally, move to a point near the player
            BaseBehaviours.FollowTarget(agent, agent.GetPlayer(), true),
            //If there are no targets, move to a random point in the roam radius
            BaseBehaviours.RoamToRandomPoint(agent)
            );

        return root;
    }
}