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

            /*
            //Moves to a target close to the player, and attacks the closest enemy to it
            BaseBehaviours.MoveToTargetWhileAttacking(agent, playerModel.modelCharacter),
            //If there are no targets, but the player is an ally, move to a point near the player
            BaseBehaviours.FollowTarget(agent, playerModel.modelCharacter.gameObject, true),
            //If there are no targets, move to a random point in the roam radius
            BaseBehaviours.RoamToRandomPoint(agent)
            */

            //TODO: If player is defensive, rush to player and attack enemies around them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Defensive),
                BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter)
                ),
            //TODO: If player is cautious, rush to player and attack enemies around them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Cautious),
                BaseBehaviours.MoveToTargetWhileAttacking(agent, playerModel.modelCharacter)
                ),
            //TODO: If player is struggling, rush to player and attack enemies around them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Panic),
                BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter)
                ),

            //Checks if the closest enemy is within melee range and makes an attack if true
            BaseBehaviours.AttackClosestTarget(agent),
            //Checks if the closest enemy is within sight range and moves towards it if true
            BaseBehaviours.MoveToClosestTarget(agent),
            //If there are no targets, but the player is an ally, move to a point near the player
            BaseBehaviours.FollowTarget(agent, agent.GetPlayer(), true),
            //If there are no targets, move to a random point in the roam radius
            BaseBehaviours.RoamToRandomPoint(agent)
            );

        return root;
    }
}
