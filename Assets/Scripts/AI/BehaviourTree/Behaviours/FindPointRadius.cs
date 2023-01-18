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

        Vector3 point = GetRandomPoint(radius);
        agent.SetDestinationPos(point);
        Debug.Log("Generated point at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }

    Vector3 GetRandomPoint(float radius)
    {
        Vector3 point = new Vector3(0, 0, 0);

        if (GetRandomPointOnNavmesh(agent.transform.position, radius, 30, out point) == false)
        {
            Debug.Log("Non navmesh");
            point = GetRandomPointNonNavmesh(radius);
        }
        else
        {
            Debug.Log("Navmesh");
        }

        return point;
    }

    Vector3 GetRandomPointNonNavmesh(float radius)
    {
        Vector3 origin = agent.transform.position;
        Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
        randomPoint.y = agent.transform.position.y;

        return randomPoint;
    }

    bool GetRandomPointOnNavmesh(Vector3 origin, float radius, int iterations, out Vector3 point)
    {
        point = new Vector3(0, 0, 0);
        for (int i = 0; i < iterations; i++)
        {
            Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
            randomPoint.y = agent.transform.position.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, agent.distanceAllowance, NavMesh.AllAreas))
            {
                point = hit.position;
                return true;
            }
        }
        return false;
    }
}
