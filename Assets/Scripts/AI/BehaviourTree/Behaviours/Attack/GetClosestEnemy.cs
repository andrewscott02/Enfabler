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
        CharacterController enemy = GetEnemy();
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
            Debug.Log("Failed to get enemy");

            state = NodeState.Failure;
        }

        return state;
    }

    CharacterController GetEnemy()
    {
        CharacterController closestCharacter = null;
        float closestDistance = 99999;

        foreach (var item in AIManager.instance.GetEnemyTeam(agent))
        {
            if (item.invisible)
                break;

            float itemDistance = Vector3.Distance(agent.gameObject.transform.position, item.gameObject.transform.position);

            //Debug.Log(item.gameObject.name + " is " + itemDistance);

            if (itemDistance < sightRadius && itemDistance < closestDistance)
            {
                closestCharacter = item;
                closestDistance = itemDistance;
            }
        }

        return closestCharacter;
    }


}
