using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class FollowBehaviourTree : BehaviourTree
{
    public ConstructPlayerModel playerModel;

    protected override Node SetupTree()
    {
        //Debug.Log("Setting up adaptive BT for " + agent.name);

        Node root = new Selector(

            //If agent is too far away from model character, rush to a distance within range
            new Sequence(
                new CheckOutDistance(agent, playerModel),
                new Selector(
                    BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter, agent.distanceAllowance)
                    )
                ),
            //If there are no targets, but the player is an ally, move to a point near the player
            BaseBehaviours.FollowTarget(agent, agent.GetPlayer(), true, agent.distanceAllowance),
            //If there are no targets, move to a random point in the roam radius
            BaseBehaviours.RoamToRandomPoint(agent, agent.distanceAllowance)
            );

        return root;
    }
}