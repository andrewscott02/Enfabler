using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class GetClosestEnemy : Node
{
    public AIController agent;
    public float sightRadius;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public GetClosestEnemy(AIController agent, float radius)
    {
        this.agent = agent;
        this.sightRadius = radius;
    }

    public override NodeState Evaluate()
    {
        CharacterController enemy = HelperFunctions.GetClosestEnemy(agent, agent.transform.position, sightRadius, false);
        if (enemy != null)
        {
            agent.SetDestinationPos(enemy.transform.position);
            //Debug.Log("Generated point at: " + enemy.transform.position);

            agent.currentTarget = enemy;
            agent.roaming = false;
            state = NodeState.Success;
        }
        else
        {
            //Debug.Log("Failed to get enemy");

            state = NodeState.Failure;
        }

        return state;
    }
}
