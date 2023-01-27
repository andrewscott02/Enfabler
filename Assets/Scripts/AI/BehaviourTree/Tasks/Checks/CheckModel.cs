using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckModel : Node
{
    public ConstructPlayerModel characterModel;
    public Descriptor descriptor;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public CheckModel(ConstructPlayerModel characterModel, Descriptor descriptor)
    {
        this.characterModel = characterModel;
        this.descriptor = descriptor;
    }

    public override NodeState Evaluate()
    {
        NodeState state = NodeState.Failure;

        if(characterModel != null)
        {
            if (characterModel.playerState == descriptor) { state = NodeState.Success; }
        }

        return state;
    }
}