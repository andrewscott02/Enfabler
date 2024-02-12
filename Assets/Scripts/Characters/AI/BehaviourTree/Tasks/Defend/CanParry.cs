using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CanParry : Node
{
    public AIController agent;

    /// <summary>
    /// Checks if an agent is able to take the parry action
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CanParry(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.CanBlock())
        {
            state = NodeState.Success;
        }
        else
        {
            state = NodeState.Failure;
        }

        return state;
    }
}
