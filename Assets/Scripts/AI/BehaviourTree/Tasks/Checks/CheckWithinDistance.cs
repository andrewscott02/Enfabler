using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckWithinDistance : Node
{
    public AIController agent;
    public ConstructPlayerModel characterModel;
    public float distance;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public CheckWithinDistance(AIController agent, ConstructPlayerModel characterModel, float distance)
    {
        this.agent = agent;
        this.characterModel = characterModel;
        this.distance = distance;
    }

    public override NodeState Evaluate()
    {
        NodeState state = NodeState.Failure;

        if(characterModel != null)
        {
            if (Vector3.Distance(agent.transform.position, characterModel.modelCharacter.transform.position) <= agent.maxDistanceFromModelCharacter)
                state = NodeState.Success;
        }

        return state;
    }
}