using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class MeleeAttack : Node
{
    public AIController agent;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
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
