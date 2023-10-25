using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class IsValidAttack : Node
{
    AIController.AIAttackData attack;
    public CharacterCombat.AttackType attackType;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public IsValidAttack(AIController.AIAttackData attack, CharacterCombat.AttackType attackType)
    {
        this.attack = attack;
        this.attackType = attackType;
    }

    public override NodeState Evaluate()
    {
        if (attack.attackType == attackType)
        {
            //Debug.Log("Can attack : " + attackType);
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}
