using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class FindPointNearTarget : Node
{
    public AIController agent;
    public BaseCharacterController target;
    public bool requiresSameTeam;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="target">The target the agent will stay near</param>
    /// <param name="requiresSameTeam">Whether this requires the agent and intercept target to be on the same team</param>
    public FindPointNearTarget(AIController agent, GameObject target, bool requiresSameTeam)
    {
        this.agent = agent;
        this.target = target.GetComponent<BaseCharacterController>();
        this.requiresSameTeam = requiresSameTeam;
    }

    public override NodeState Evaluate()
    {
        if (target == null) { return NodeState.Failure; }

        if (requiresSameTeam && AIManager.instance.OnSameTeam(agent, target) == false) { return NodeState.Failure; }

        Vector3 point = target.transform.position + (target.transform.TransformDirection(agent.followVector) * agent.followDistance);

        agent.SetDestinationPos(point);
        //Debug.Log("Generated point near target at: " + point);

        agent.alert = false;
        state = NodeState.Success;
        return state;
    }
}
