using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckPanic : Node
{
    public ConstructPlayerModel characterModel;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public CheckPanic(ConstructPlayerModel characterModel)
    {
        this.characterModel = characterModel;
    }

    public override NodeState Evaluate()
    {
        NodeState state = NodeState.Failure;

        if(characterModel != null)
        {
            if (characterModel.playerState == Descriptor.Panic) { state = NodeState.Success; }
        }

        return state;
    }
}