using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class EnqueueAttack : Node
{
    AIController agent;
    public CharacterCombat.AttackType attackType;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public EnqueueAttack(AIController agent, CharacterCombat.AttackType attackType)
    {
        this.agent = agent;
        this.attackType = attackType;
    }

    public override NodeState Evaluate()
    {
        if (agent.CanAttack(attackType))
        {
            AIManager.instance.Enqueue(agent);

            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}
