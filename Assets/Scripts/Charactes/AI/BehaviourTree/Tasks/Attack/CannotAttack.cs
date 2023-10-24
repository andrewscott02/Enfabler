using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CannotAttack : Node
{
    public AIController agent;
    public CharacterCombat.AttackType attackType;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CannotAttack(AIController agent, CharacterCombat.AttackType attackType)
    {
        this.agent = agent;
        this.attackType = attackType;
    }

    public override NodeState Evaluate()
    {
        if (agent.CanAttack(attackType))
        {
            //Debug.Log("Can attack");
            return NodeState.Failure;
        }

        return NodeState.Success;
    }
}
