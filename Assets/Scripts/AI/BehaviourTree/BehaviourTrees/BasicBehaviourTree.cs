using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class BasicBehaviourTree : BehaviourTree
{
    protected override Node SetupTree()
    {
        //Debug.Log("Setting up basic BT for " + agent.name);

        Node root = new Selector(
            //Checks if the closest enemy is within melee range and makes an attack if true
            new Sequence(
                new GetClosestEnemy(agent, agent.meleeDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f),
                new MeleeAttack(agent, agent.currentTarget)
                ),
            //Checks if the closest enemy is within sight range and moves towards it if true
            new Sequence(
                new GetClosestEnemy(agent, agent.sightDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f)
                ),
            //If there are no targets, but the player is an ally, move to a point near the player
            new Sequence(
                new FindPointNearTarget(agent, agent.GetPlayer(), agent.followDistance, true),
                new MoveToDestination(agent, agent.distanceAllowance, Mathf.Infinity)),
            //If there are no targets, move to a random point in the roam radius
            new Sequence(
                new FindPointRadius(agent, agent.roamDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f)
                )
            );

        return root;
    }
}
