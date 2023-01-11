using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

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
        /*
        UnityEngine.AI.NavMeshHit point;
        agent.GetNavMeshAgent().SamplePathPosition(0, radius, out point);
        agent.SetDestinationPos(point.position);

        Debug.Log("Generated point at: " + point.position);
        */

        if (agent.roaming)
            return NodeState.Running;

        Vector3 point = GetRandomPoint(radius);
        agent.SetDestinationPos(point);
        Debug.Log("Generated point at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }

    Vector3 GetRandomPoint(float radius)
    {
        Vector3 origin = agent.transform.position;
        float randX = Random.Range(-360, 360);
        float randY = 0;
        float randZ = Random.Range(-360, 360);
        Vector3 direction = new Vector3(randX, randY, randZ);
        direction.Normalize();

        float distance = Random.Range(0, radius);
        Vector3 point = origin + (direction * distance);

        return point;
    }
}
