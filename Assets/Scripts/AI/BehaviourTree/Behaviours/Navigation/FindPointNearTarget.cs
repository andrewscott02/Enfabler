using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class FindPointNearTarget : Node
{
    public AIController agent;
    public CharacterController target;
    public float radius;
    public bool requiresSameTeam;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public FindPointNearTarget(AIController agent, GameObject target, float radius, bool requiresSameTeam)
    {
        this.agent = agent;
        this.target = target.GetComponent<CharacterController>();
        this.radius = radius;
        this.requiresSameTeam = requiresSameTeam;
    }

    public override NodeState Evaluate()
    {
        if (requiresSameTeam)
        {
            if (AIManager.instance.OnSameTeam(agent, target) == false) { return NodeState.Failure; }
        }

        if (target == null) { return NodeState.Failure; }

        Vector3 point = target.transform.position + (target.transform.TransformDirection(agent.followVector) * radius);

        agent.SetDestinationPos(point);
        //Debug.Log("Generated point near target at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }
}
