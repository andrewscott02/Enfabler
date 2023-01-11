using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class MoveToDestination : Node
{
    public AIController agent;
    public float distanceAllowance;

    float maxTime;
    float elapsedTime;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public MoveToDestination(AIController agent, float distanceAllowance, float maxTime)
    {
        this.agent = agent;
        this.distanceAllowance = distanceAllowance;
        this.maxTime = maxTime;
    }

    public override NodeState Evaluate()
    {
        if (agent.NearDestination(distanceAllowance))
        {
            state = NodeState.Success;
            Debug.Log("Arrived at destination: " + agent.GetDestination());
            elapsedTime = 0;
            agent.roaming = false;
            return state;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxTime)
            {
                state = NodeState.Failure;
                Debug.Log("Failed to arrive at destination: " + agent.GetDestination());
                elapsedTime = 0;
                agent.roaming = false;
                return state;
            }
            else
            {
                state = NodeState.Running;
                Debug.Log("Moving from " + agent.transform.position + " to " + agent.GetDestination());
                agent.MoveToDestination();
            }

            return state;
        }
    }
}
