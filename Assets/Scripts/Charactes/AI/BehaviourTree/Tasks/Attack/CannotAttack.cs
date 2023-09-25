using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CannotAttack : Node
{
    public AIController agent;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CannotAttack(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.CanAttack())
        {
            Debug.Log("Can attack");
            return NodeState.Failure;
        }

        return NodeState.Success;
    }
}
