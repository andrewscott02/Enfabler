using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckNotActive: Node
{
    public AIController agent;

    /// <summary>
    /// Checks that the agent is active
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CheckNotActive(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        NodeState state = NodeState.Success;

        if(agent.activeAgent)
        {
            state = NodeState.Failure;
        }

        return state;
    }
}