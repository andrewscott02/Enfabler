using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;

public class FindPointRadius : Node
{
    public AIController agent;
    int iterations = 30;
    Vector3 roamCenter;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public FindPointRadius(AIController agent)
    {
        this.agent = agent;
        roamCenter = agent.transform.position;
    }

    public override NodeState Evaluate()
    {
        agent.alert = agent.roamTimeElapsed <= 15f;

        HelperFunctions.GetRandomPoint(agent.alert ? agent.transform.position : roamCenter, agent.GetRoamDistance(), agent.distanceAllowance, iterations, out Vector3 point);
        agent.SetDestinationPos(point);
        //Debug.Log("Generated point at: " + point);

        state = NodeState.Success;
        return state;
    }
}
