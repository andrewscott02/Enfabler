using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class IsTurn : Node
{
    AIController agent;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public IsTurn(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (AIManager.instance.CanAttack(agent))
        {
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}
