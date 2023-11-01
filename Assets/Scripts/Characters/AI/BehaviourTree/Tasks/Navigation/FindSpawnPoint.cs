using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class FindSpawnPoint : Node
{
    public AIController agent;
    int iterations = 30;
    float radius;
    Vector3 spawnPoint;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public FindSpawnPoint(AIController agent)
    {
        this.agent = agent;
        spawnPoint = agent.transform.position;
    }

    public override NodeState Evaluate()
    {
        agent.alert = agent.roamTimeElapsed <= 15f;
        float radius = agent.alert ? this.radius : 0f;
        float distanceAllowance = agent.alert ? agent.distanceAllowance : 0.5f;

        Vector3 point = HelperFunctions.GetRandomPoint(agent.alert ? agent.transform.position : spawnPoint, radius, distanceAllowance, iterations);
        agent.SetDestinationPos(point);
        //Debug.Log("Generated point at: " + point);

        state = NodeState.Success;
        return state;
    }
}
