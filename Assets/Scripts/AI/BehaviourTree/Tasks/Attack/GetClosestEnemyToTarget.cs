using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class GetClosestEnemyToTarget : Node
{
    public AIController agent;
    public GameObject target;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public GetClosestEnemyToTarget(AIController agent, GameObject target)
    {
        this.agent = agent;
        this.target = target;
    }

    public override NodeState Evaluate()
    {
        CharacterController enemy = HelperFunctions.GetClosestEnemy(agent, target.transform.position, 99999999, true);
        if (enemy != null)
        {
            agent.SetDestinationPos(enemy.transform.position);
            Debug.Log("Generated point at near target: " + enemy.name);

            agent.currentTarget = enemy;
            agent.roaming = false;
            state = NodeState.Success;
        }
        else
        {
            Debug.Log("Failed to get enemy near target");

            state = NodeState.Failure;
        }

        return state;
    }
}
