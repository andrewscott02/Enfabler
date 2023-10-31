using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckNotActiveOnStart: Node
{
    public AIController agent;

    /// <summary>
    /// Checks that the agent is active
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CheckNotActiveOnStart(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        NodeState state = NodeState.Success;

        if(agent.activeOnStart)
        {
            state = NodeState.Failure;
        }

        return state;
    }
}