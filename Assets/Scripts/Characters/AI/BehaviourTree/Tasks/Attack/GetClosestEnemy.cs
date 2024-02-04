using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class GetClosestEnemy : Node
{
    public AIController agent;
    public float overrideDistance;

    /// <summary>
    /// Commands an agent to get the closest enemy
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="radius">The maximum distance an enemy can be before it is ignored</param>
    public GetClosestEnemy(AIController agent, float overrideDistance = -1)
    {
        this.agent = agent;
        this.overrideDistance = overrideDistance;
    }

    public override NodeState Evaluate()
    {
        float distance = overrideDistance < 0 ? agent.GetSightDistance() : overrideDistance;

        BaseCharacterController enemy = HelperFunctions.GetClosestEnemy(agent, agent.transform.position, distance, false);
        if (enemy != null)
        {
            agent.SetDestinationPos(enemy.transform.position);
            //Debug.Log("Generated point");
            //Debug.Log("Generated point at: " + enemy.transform.position);

            agent.currentTarget = enemy;
            agent.alert = true;

            state = NodeState.Success;

            AIManager.instance.AddEnemy(agent);
        }
        else
        {
            //Debug.Log("Failed to get enemy nearby");
            AIManager.instance.Dequeue(agent, false);
            AIManager.instance.RemoveEnemy(agent);
            state = NodeState.Failure;
        }

        return state;
    }
}
