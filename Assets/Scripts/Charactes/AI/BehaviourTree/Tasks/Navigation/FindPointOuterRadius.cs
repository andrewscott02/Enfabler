using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class FindPointOuterRadius : Node
{
    public AIController agent;
    float radius;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public FindPointOuterRadius(AIController agent, float radius)
    {
        this.agent = agent;
        this.radius = radius;
    }

    public override NodeState Evaluate()
    {
        if (agent.roaming)
            return NodeState.Running;

        Vector3 direction = Random.insideUnitSphere.normalized;
        direction.y = 0;

        Vector3 point = agent.currentTarget.transform.position + (direction * radius);

        agent.SetDestinationPos(point);
        //Debug.Log("Generated point at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }
}
