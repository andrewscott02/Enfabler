using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class Roam : Node
{
    public AIController agent;
    public float radius;
    public Vector3 point;
    public float distanceAllowance;

    float maxTime;
    float elapsedTime;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public Roam(AIController agent, float radius, float distanceAllowance, float maxTime)
    {
        this.agent = agent;
        this.radius = radius;
        this.distanceAllowance = distanceAllowance;
        this.maxTime = maxTime;

        agent.SetDestinationPos(GetRandomPoint(this.radius));
    }

    public override NodeState Evaluate()
    {
        if (agent.NearDestination(distanceAllowance))
        {
            state = NodeState.Success;
            Debug.Log("Arrived at destination: " + point);
            elapsedTime = 0;
            agent.SetDestinationPos(GetRandomPoint(radius));
            return state;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxTime)
            {
                state = NodeState.Failure;
                Debug.Log("Arrived at destination: " + point);
                elapsedTime = 0;
                agent.SetDestinationPos(GetRandomPoint(radius));
                return state;
            }
            else
            {
                state = NodeState.Running;
                Debug.Log("Moving from " + agent.transform.position + " to " + point);
            }

            return state;
        }
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
