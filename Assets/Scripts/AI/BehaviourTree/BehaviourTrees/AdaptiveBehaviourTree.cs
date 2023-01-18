using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class AdaptiveBehaviourTree : BehaviourTree
{
    public ConstructPlayerModel playerModel;

    protected override Node SetupTree()
    {
        //Debug.Log("Setting up adaptive BT for " + agent.name);

        Node root = new Selector(
            /*
            //TODO: If player is being aggressive, focus on enemies they are not targetting
            new Sequence(
                new CheckAggressive(playerModel),
                new GetClosestEnemy(agent, agent.meleeDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f),
                new MeleeAttack(agent, agent.currentTarget)
                ),
            //TODO: If player is being defensive, rush to player and attack enemies around them
            new Sequence(
                new GetClosestEnemy(agent, agent.meleeDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f),
                new MeleeAttack(agent, agent.currentTarget)
                ),
            //TODO: If player is mobile, move slowly to player and attack enemies around them
            new Sequence(
                new GetClosestEnemy(agent, agent.meleeDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f),
                new MeleeAttack(agent, agent.currentTarget)
                ),
            //TODO: If player is countering, flank their target and attack
            new Sequence(
            new GetClosestEnemy(agent, agent.meleeDistance),
            new MoveToDestination(agent, agent.distanceAllowance, 6f),
            new MeleeAttack(agent, agent.currentTarget)
            ),
            //TODO: If player is struggling, rush to player and attack enemies around them
            new Sequence(
                new GetClosestEnemy(agent, agent.meleeDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f),
                new MeleeAttack(agent, agent.currentTarget)
                ),
            */

            //Moves to a target close to the player, and attacks the closest enemy to it
            new Sequence(
                new GetClosestEnemyToTarget(agent, playerModel.modelCharacter),
                new MoveToDestination(agent, agent.distanceAllowance, 6f),
                new GetClosestEnemy(agent, agent.sightDistance),
                new MeleeAttack(agent, agent.currentTarget)
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
