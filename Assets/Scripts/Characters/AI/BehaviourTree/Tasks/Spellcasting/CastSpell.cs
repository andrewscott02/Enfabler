using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CastSpell : Node
{
    AIController agent;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public CastSpell(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        agent.CastSpell();

        return NodeState.Success;
    }
}