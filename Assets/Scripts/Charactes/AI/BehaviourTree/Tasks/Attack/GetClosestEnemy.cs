using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class GetClosestEnemy : Node
{
    public AIController agent;
    public float sightRadius;

    /// <summary>
    /// Commands an agent to get the closest enemy
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="radius">The maximum distance an enemy can be before it is ignored</param>
    public GetClosestEnemy(AIController agent, float radius)
    {
        this.agent = agent;
        this.sightRadius = radius;
    }

    public override NodeState Evaluate()
    {
        BaseCharacterController enemy = HelperFunctions.GetClosestEnemy(agent, agent.transform.position, sightRadius, false);
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
            Debug.Log("Failed to get enemy within " + sightRadius);

            state = NodeState.Failure;
        }

        return state;
    }
}
