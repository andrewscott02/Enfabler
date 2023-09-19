using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class BeingAttacked : Node
{
    public AIController agent;

    /// <summary>
    /// Checks if the agent is being attacked by at least 1 enemy
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public BeingAttacked(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.GetCharacterCombat().GetTargetted())
        {
            //Debug.Log("Being attacked");
            state = NodeState.Success;
        }
        else
        {
            //Debug.Log("Not being attacked");
            state = NodeState.Failure;
        }

        return state;
    }
}
