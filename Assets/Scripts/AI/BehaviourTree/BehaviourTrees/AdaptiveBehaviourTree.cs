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

            //If agent is too far away from model character, rush to a distance within range
            new Sequence(
                new CheckOutDistance(agent, playerModel),
                new Selector(
                    BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter)
                    )
                ),

        #region Adaptive Behaviours

            //If player is aggressive, focus on enemies they are not targetting
            new Sequence(
                new CheckModel(playerModel, Descriptor.Aggressive),
                new Selector(
                    BaseBehaviours.IgnoreModelTargets(agent, playerModel),
                    //If player is targetting all available targets, move to closest target and attack
                    BaseBehaviours.AttackClosestTarget(agent),
                    BaseBehaviours.MoveToClosestTarget(agent)
                    )
                ),
            //If player is counterring, move slowly to player and attack enemies around them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Counter),
                BaseBehaviours.MoveToTargetWhileAttacking(agent, playerModel.modelCharacter, agent.sightDistance)
                ),
            //If player is defensive, rush to player and attack enemies around them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Defensive),
                BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter)
                ),
            //If player is cautious, rush to player and attack enemies around them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Cautious),
                BaseBehaviours.FlankTarget(agent, playerModel, agent.meleeDistance, true, true)
                ),
            //If player is struggling, draw enemies away from them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Panic),
                BaseBehaviours.InterceptTarget(agent, playerModel, agent.meleeDistance, true, true)
                ),

        #endregion

        #region General Behaviours - In case the model is null or has insufficient information

            //Checks if the closest enemy is within melee range and makes an attack if true
            BaseBehaviours.AttackClosestTarget(agent),
            //Checks if the closest enemy is within sight range and moves towards it if true
            BaseBehaviours.MoveToClosestTarget(agent),

        #endregion

        #region Idle Behaviours - When there are no enemies

            //If there are no targets, but the player is an ally, move to a point near the player
            BaseBehaviours.FollowTarget(agent, agent.GetPlayer(), true),
            //If there are no targets, move to a random point in the roam radius
            BaseBehaviours.RoamToRandomPoint(agent)

        #endregion

            );

        return root;
    }
}