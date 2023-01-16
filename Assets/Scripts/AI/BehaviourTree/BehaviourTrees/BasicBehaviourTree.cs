using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class BasicBehaviourTree : BehaviourTree
{
    protected override Node SetupTree()
    {
        //Node root = new Roam(agent, 30f, 1f, 6f);

        Node root = new Selector(
            //Checks if the closest enemy is within melee range and makes an attack if true
            new Sequence(
            new GetClosestEnemy(agent, agent.meleeDistance),
            new MeleeAttack(agent, agent.currentTarget)
            ),
            //Checks if the closest enemy is within sight range and moves towards it if true
            new Sequence(new List<Node>
            {
            new GetClosestEnemy(agent, agent.sightDistance),
            new MoveToDestination(agent, 1f, 6f)
            }),
            //If there are no targets, move to a random point in the roam radius
            new Sequence(new List<Node>
            {
            new FindPointRadius(agent, agent.roamDistance),
            new MoveToDestination(agent, 1f, 6f)
            })
        );

        return root;
    }
}
