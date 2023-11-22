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

            BaseBehaviours.CastSpell(agent),

            new Sequence(
                //If they are able to make a ranged attack
                new CanAttack(agent, CharacterCombat.AttackType.SecondaryAttack),
                //Checks if the closest enemy is within ranged distance and makes an attack if true
                BaseBehaviours.AttackClosestTarget(agent, true, agent.GetAttackFromType(CharacterCombat.AttackType.SecondaryAttack), CharacterCombat.AttackType.SecondaryAttack)
                ),

            new Sequence(
                //If they are able to make a switch melee attack
                new CanAttack(agent, CharacterCombat.AttackType.SwitchPrimaryAttack),
                //Checks if the closest enemy is within switch melee distance and makes an attack if true
                BaseBehaviours.AttackClosestTarget(agent, true, agent.GetAttackFromType(CharacterCombat.AttackType.SwitchPrimaryAttack), CharacterCombat.AttackType.SwitchPrimaryAttack)
                ),

            new Sequence(
                //If they are able to make a switch ranged attack
                new CanAttack(agent, CharacterCombat.AttackType.SwitchSecondaryAttack),
                //Checks if the closest enemy is within switch melee distance and makes an attack if true
                BaseBehaviours.AttackClosestTarget(agent, true, agent.GetAttackFromType(CharacterCombat.AttackType.SwitchSecondaryAttack), CharacterCombat.AttackType.SwitchSecondaryAttack)
                ),

            new Selector(

                new Sequence(
                    //If they are unable to make an attack, move to a point a short distance away
                    new CannotAttack(agent, CharacterCombat.AttackType.PrimaryAttack),
                    BaseBehaviours.MoveToRange(agent, 35f, false)
                    ),
                
                //Checks if the closest enemy is within melee range and makes an attack if true
                BaseBehaviours.AttackClosestTarget(agent, true, agent.GetAttackFromType(CharacterCombat.AttackType.PrimaryAttack), CharacterCombat.AttackType.PrimaryAttack)
                ),

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