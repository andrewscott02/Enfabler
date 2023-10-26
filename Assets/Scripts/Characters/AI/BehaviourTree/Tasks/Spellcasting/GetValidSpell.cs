using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class GetValidSpell : Node
{
    AIController agent;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public GetValidSpell(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        AIController.AISpellData spellData = agent.GetValidSpell();

        if (spellData.spell == null)
            return NodeState.Failure;

        if (agent.PrepareSpell(spellData))
            return NodeState.Success;

        return NodeState.Failure;
    }
}