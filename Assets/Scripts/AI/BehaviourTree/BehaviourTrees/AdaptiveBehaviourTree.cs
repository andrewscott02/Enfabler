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

        #region Adaptive Behaviours

            //If player is aggressive, focus on enemies they are not targetting
            new Sequence(
                new CheckModel(playerModel, Descriptor.Aggressive),
                BaseBehaviours.IgnoreModelTargets(agent, playerModel)
                ),
            //TODO: If player is counterring, flank their target and attack
            new Sequence(
                new CheckModel(playerModel, Descriptor.Counter),
                BaseBehaviours.MoveToTargetWhileAttacking(agent, playerModel.modelCharacter.gameObject)
                ),
            //If player is defensive, move slowly to player and attack enemies around them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Defensive),
                BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter.gameObject)
                ),
            //If player is cautious, move slowly to player and attack enemies around them
            new Sequence(
                new CheckModel(playerModel, Descriptor.Cautious),
                BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter.gameObject)
                ),
            //If player is struggling, rush to player and draw enemies away
            new Sequence(
                new CheckModel(playerModel, Descriptor.Panic),
                BaseBehaviours.FlankTarget(agent, playerModel, 6f, true)
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