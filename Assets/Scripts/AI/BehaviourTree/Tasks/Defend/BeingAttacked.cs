using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class BeingAttacked : Node
{
    public AIController agent;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public BeingAttacked(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.GetCharacterCombat().GetTargetted())
        {
            Debug.Log("Being attacked");
            state = NodeState.Success;
        }
        else
        {
            Debug.Log("Not being attacked");
            state = NodeState.Failure;
        }

        return state;
    }
}
