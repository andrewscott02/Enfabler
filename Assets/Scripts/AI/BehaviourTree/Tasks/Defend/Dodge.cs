using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class Dodge : Node
{
    public AIController agent;
    public float distanceAllowance;

    /// <summary>
    /// Commands an agent to dodge. If the target is near its current destination, this will fail.
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="distanceAllowance">The distance the agent can be from its destination for this to fail</param>
    public Dodge(AIController agent, float distanceAllowance)
    {
        this.agent = agent;
        this.distanceAllowance = distanceAllowance;
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
