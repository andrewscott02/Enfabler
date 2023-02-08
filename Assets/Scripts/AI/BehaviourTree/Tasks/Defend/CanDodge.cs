using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CanDodge : Node
{
    public AIController agent;
    float cooldown;
    float elapsedTime;

    /// <summary>
    /// Checks if an agent is able to take the dodge action
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CanDodge(AIController agent)
    {
        this.agent = agent;
        cooldown = agent.dodgeCooldown;
    }

    public override NodeState Evaluate()
    {
        if (elapsedTime > cooldown)
        {
            elapsedTime = 0;

            if (agent.CanDodge())
            {
                state = NodeState.Success;
            }
            else
            {
                state = NodeState.Failure;
            }
        }

        elapsedTime += Time.deltaTime;

        return state;
    }
}
