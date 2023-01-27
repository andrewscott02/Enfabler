using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class Parry : Node
{
    public AIController agent;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public Parry(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.GetCharacterCombat().canAttack)
        {
            agent.GetCharacterCombat().Parry();
            state = NodeState.Success;
        }
        else
        {
            state = NodeState.Failure;
        }

        return state;
    }
}
