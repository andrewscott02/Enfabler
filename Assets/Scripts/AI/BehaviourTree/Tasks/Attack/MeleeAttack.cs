using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class MeleeAttack : Node
{
    public AIController agent;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public MeleeAttack(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Attack state is attempted at:" + agent.currentTarget.name);
        bool attackSuccess = agent.AttackTarget();
        if (attackSuccess)
        {
            //Debug.Log("Attack state is successful");
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}
