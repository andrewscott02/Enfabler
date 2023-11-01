using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class GetModelNonTarget : Node
{
    public AIController agent;
    public ConstructPlayerModel model;

    /// <summary>
    /// Commands an agent to get the closest enemy its model character is not targetting
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="model">The agent model used for decision making</param>
    public GetModelNonTarget(AIController agent, ConstructPlayerModel model)
    {
        this.agent = agent;
        this.model = model;
    }

    public override NodeState Evaluate()
    {
        BaseCharacterController enemy = HelperFunctions.GetClosestEnemyExcludingList(agent, agent.transform.position, 99999999, true, model.currentTargets);
        if (enemy != null)
        {
            agent.SetDestinationPos(enemy.transform.position);
            //Debug.Log("Generated point at near target: " + enemy.name);

            agent.currentTarget = enemy;
            agent.alert = true;
            state = NodeState.Success;
        }
        else
        {
            //Debug.Log("Failed to get enemy near target");

            state = NodeState.Failure;
        }

        return state;
    }
}
