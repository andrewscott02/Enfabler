using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class GetValidAttack : Node
{
    AIController agent;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public GetValidAttack(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
       int attackIndex = agent.GetValidAttack();

        if (attackIndex < 0)
        {
            //Debug.Log("Valid attack not found: " + attackIndex);
            return NodeState.Failure;
        }

        //Debug.Log("Valid attack found: " + attackIndex);
        agent.preparedAttack = attackIndex;
        return NodeState.Success;
    }
}