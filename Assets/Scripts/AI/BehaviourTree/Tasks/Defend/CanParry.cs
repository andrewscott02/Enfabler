using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CanParry : Node
{
    public AIController agent;
    float cooldown;
    float elapsedTime;

    /// <summary>
    /// Checks if an agent is able to take the parry action
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CanParry(AIController agent)
    {
        this.agent = agent;
        cooldown = agent.parryCooldown;
    }

    public override NodeState Evaluate()
    {
        if (elapsedTime > cooldown)
        {
            elapsedTime = 0;

            if (agent.CanParry())
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
