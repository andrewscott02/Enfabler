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
        if (agent.roaming)
            return NodeState.Running;

        Vector3 point = HelperFunctions.GetRandomPoint(agent.roamTimeElapsed > 15f ? spawnPoint : agent.transform.position, radius, agent.distanceAllowance, iterations);
        agent.SetDestinationPos(point);
        //Debug.Log("Generated point at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }
}
