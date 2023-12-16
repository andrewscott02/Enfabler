using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class GetClosestEnemyWithinAttack : Node
{
    public AIController agent;

    /// <summary>
    /// Commands an agent to get the closest enemy
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="radius">The maximum distance an enemy can be before it is ignored</param>
    public GetClosestEnemyWithinAttack(AIController agent)
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        float distance = agent.attacks[agent.preparedAttack].distance;

        BaseCharacterController enemy = HelperFunctions.GetClosestEnemy(agent, agent.transform.position, distance, false);
        if (enemy != null)
        {
            agent.SetDestinationPos(enemy.transform.position);
            //Debug.Log("Generated point");
            //Debug.Log("Generated point at: " + enemy.transform.position);

            agent.currentTarget = enemy;
            agent.alert = true;

            state = NodeState.Success;
        }
        else
        {
            //Debug.Log("Failed to get enemy nearby");
            AIManager.instance.Dequeue(agent, false);
            state = NodeState.Failure;
        }

        return state;
    }
}
