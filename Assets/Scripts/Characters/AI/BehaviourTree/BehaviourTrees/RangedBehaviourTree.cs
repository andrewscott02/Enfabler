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

            BaseBehaviours.AttackClosestTargetNoMoving(agent),

            //Checks if the closest enemy is within sight range and moves at a range away from it
            BaseBehaviours.MoveToRange(agent, 35f, false),
            //If there are no targets, but the player is an ally, move to a point near the player
            BaseBehaviours.FollowPing(agent, agent.GetPlayer()),
            //If there are no targets, move to a random point in the roam radius
            BaseBehaviours.RoamToRandomPoint(agent)
            );

        return root;
    }
}