using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class FindPointRadius : Node
{
    public AIController agent;
    int iterations = 30;
    float radius;
    Vector3 roamCenter;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public FindPointRadius(AIController agent, float radius)
    {
        this.agent = agent;
        this.radius = radius;
        roamCenter = agent.transform.position;
    }

    public override NodeState Evaluate()
    {
        if (agent.roaming)
            return NodeState.Running;

        Vector3 point = HelperFunctions.GetRandomPoint(agent.roamTimeElapsed > 15f ? roamCenter : agent.transform.position, radius, agent.distanceAllowance, iterations);
        agent.SetDestinationPos(point);
        //Debug.Log("Generated point at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }
}
