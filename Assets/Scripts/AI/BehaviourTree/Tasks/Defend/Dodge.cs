using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class Dodge : Node
{
    public AIController agent;
    public float distanceAllowance;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public Dodge(AIController agent, float distanceAllowance)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.NearDestination(distanceAllowance) == false && agent.GetCharacterCombat().canAttack)
        {
            agent.GetCharacterCombat().Dodge();
            state = NodeState.Success;
        }
        else
        {
            state = NodeState.Failure;
        }

        return state;
    }
}
