using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class DeactivateAgent : Node
{
    public AIController agent;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public DeactivateAgent(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        agent.Deactivate();
        return NodeState.Success;
    }
}