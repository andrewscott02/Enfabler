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

            new Selector(

                new Sequence(
                    //If they are unable to make an attack, move to a point a short distance away
                    new CannotAttack(agent, CharacterCombat.AttackType.SecondaryAttack),
                    BaseBehaviours.MoveToRange(agent, 35f, false)
                    ),

                    //Checks if the closest enemy is within melee range and makes an attack if true
                    BaseBehaviours.AttackClosestTarget(agent, true, agent.GetAttackFromType(CharacterCombat.AttackType.SecondaryAttack), CharacterCombat.AttackType.SecondaryAttack)
                ),
            
            //Checks if the closest enemy is within melee range and makes an attack if true
            BaseBehaviours.AttackClosestTarget(agent, true, agent.GetAttackFromType(CharacterCombat.AttackType.PrimaryAttack), CharacterCombat.AttackType.PrimaryAttack),
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