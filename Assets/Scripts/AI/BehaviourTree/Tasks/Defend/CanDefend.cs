using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CanDefend : Node
{
    public AIController agent;

    /// <summary>
    /// Checks if an agent is able to take defensive actions
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CanDefend(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.CanDefend())
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
