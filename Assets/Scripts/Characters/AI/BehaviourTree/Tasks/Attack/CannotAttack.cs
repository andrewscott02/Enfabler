using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using Enfabler.Attacking;

public class CannotAttack : Node
{
    public AIController agent;
    public E_AttackType attackType;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CannotAttack(AIController agent, E_AttackType attackType)
    {
        this.agent = agent;
        this.attackType = attackType;
    }

    public override NodeState Evaluate()
    {
        if (agent.CanAttack(agent.preparedAttack))
        {
            //Debug.Log("Can attack");
            return NodeState.Failure;
        }

        return NodeState.Success;
    }
}
