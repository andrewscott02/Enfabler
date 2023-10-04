using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class Parry : Node
{
    public AIController agent;

    /// <summary>
    /// Commands an agent to parry
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public Parry(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (agent.GetCharacterCombat().canDodge)
        {
            agent.GetCharacterCombat().Block(true);
            state = NodeState.Success;
        }
        else
        {
            agent.GetCharacterCombat().Block(false);
            state = NodeState.Failure;
        }

        return state;
    }
}
