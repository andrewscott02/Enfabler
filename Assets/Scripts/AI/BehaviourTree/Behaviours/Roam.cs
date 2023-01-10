using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class Roam : Node
{
    public AIController agent;
    public Vector3 point;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public Roam(AIController newAgent, float radius)
    {
        agent = newAgent;

        Vector3 origin = agent.transform.position;
        float randX = Random.Range(0, 360);
        float randY = Random.Range(0, 360);
        float randZ = Random.Range(0, 360);
        Vector3 direction = new Vector3(randX, randY, randZ);
        direction.Normalize();

        float distance = Random.Range(0, radius);
        Vector3 point = origin + (direction * distance);

        agent.SetDestinationPos(point);
    }

    public override NodeState Evaluate()
    {
        if (agent.NearDestination(5f))
        {
            state = NodeState.Success;
            return state;
        }
        else
        {
            state = NodeState.Running;
            return state;
        }
    }
}
