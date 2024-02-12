using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class FindPointNearPing : Node
{
    public AIController agent;
    public BaseCharacterController target;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="target">The target the agent will stay near</param>
    /// <param name="requiresSameTeam">Whether this requires the agent and intercept target to be on the same team</param>
    public FindPointNearPing(AIController agent, GameObject target)
    {
        this.agent = agent;
        this.target = target.GetComponent<BaseCharacterController>();
    }

    public override NodeState Evaluate()
    {
        if (target == null || !agent.pinged) { return NodeState.Failure; }

        Vector3 point = target.transform.position + (target.transform.TransformDirection(agent.followVector) * agent.followDistance);

        agent.SetDestinationPos(point);
        //Debug.Log(agent.gameObject + " is following generated point near ping at: " + point);

        agent.alert = false;
        state = NodeState.Success;
        return state;
    }
}
