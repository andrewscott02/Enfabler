using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CanDodge : Node
{
    public AIController agent;

    /// <summary>
    /// Checks if an agent is able to take the dodge action
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CanDodge(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.CanDodge())
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
