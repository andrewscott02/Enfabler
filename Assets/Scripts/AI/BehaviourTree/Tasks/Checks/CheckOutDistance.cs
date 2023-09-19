using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckOutDistance : Node
{
    public AIController agent;
    public ConstructPlayerModel characterModel;
    public float distance;

    /// <summary>
    /// Checks that the agent is outside a specified distance to its model character
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    /// <param name="characterModel">The agent model used for decision making</param>
    /// <param name="distance">The max distance the agent can be from its model character</param>
    public CheckOutDistance(AIController agent, ConstructPlayerModel characterModel, float distance)
    {
        this.agent = agent;
        this.characterModel = characterModel;
        this.distance = distance;
    }

    public override NodeState Evaluate()
    {
        NodeState state = NodeState.Failure;

        if(characterModel != null && characterModel.modelCharacter != null)
        {
            if (Vector3.Distance(agent.transform.position, characterModel.modelCharacter.transform.position) > distance)
                state = NodeState.Success;
        }

        return state;
    }
}