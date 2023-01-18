using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class FindPointRadius : Node
{
    public AIController agent;
    public float radius;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public FindPointRadius(AIController agent, float radius)
    {
        this.agent = agent;
        this.radius = radius;
    }

    public override NodeState Evaluate()
    {
        if (agent.roaming)
            return NodeState.Running;

        Vector3 point = HelperFunctions.GetRandomPoint(agent.transform.position, radius, agent.distanceAllowance);
        agent.SetDestinationPos(point);
        Debug.Log("Generated point at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }
}
