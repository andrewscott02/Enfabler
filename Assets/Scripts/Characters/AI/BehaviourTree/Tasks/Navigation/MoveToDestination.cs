using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class MoveToDestination : Node
{
    public AIController agent;

    float distanceAllowance;
    float maxTime;
    float elapsedTime;

    public bool sprinting;

    /// <summary>
    /// Commands an agent to move to its current destination
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="distanceAllowance">The maximum distance the destination is allowed to be</param>
    /// <param name="maxTime">The maximum time that the agent can move before resetting its movement</param>
    /// <param name="sprinting">Whether the agent sprints to its destination</param>
    public MoveToDestination(AIController agent, float maxTime, bool sprinting, float distanceAllowance)
    {
        this.agent = agent;
        this.maxTime = maxTime;
        this.sprinting = sprinting;
        this.distanceAllowance = distanceAllowance;
    }

    public override NodeState Evaluate()
    {
        if (agent.NearDestination(distanceAllowance))
        {
            //Debug.Log("Arrived at destination: " + agent.GetDestination());
            elapsedTime = 0;
            return NodeState.Success;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxTime)
            {
                state = NodeState.Failure;
                //Debug.Log("Failed to arrive at destination: " + agent.GetDestination());
                elapsedTime = 0;
                return state;
            }
            else
            {
                state = NodeState.Running;
                //Debug.Log("Moving from " + agent.transform.position + " to " + agent.GetDestination());
                agent.MoveToDestination(sprinting);
            }

            return state;
        }
    }
}
