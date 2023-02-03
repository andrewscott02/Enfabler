using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckOutDistance : Node
{
    public AIController agent;
    public ConstructPlayerModel characterModel;

    /// <summary>
    /// Checks that the agent is outside a specified distance to its model character
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="characterModel">The agent model used for decision making</param>
    public CheckOutDistance(AIController agent, ConstructPlayerModel characterModel)
    {
        this.agent = agent;
        this.characterModel = characterModel;
    }

    public override NodeState Evaluate()
    {
        NodeState state = NodeState.Failure;

        if(characterModel != null)
        {
            if (Vector3.Distance(agent.transform.position, characterModel.modelCharacter.transform.position) > agent.maxDistanceFromModelCharacter)
                state = NodeState.Success;
        }

        return state;
    }
}