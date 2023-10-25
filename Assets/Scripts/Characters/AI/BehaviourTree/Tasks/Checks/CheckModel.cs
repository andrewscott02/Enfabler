using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckModel : Node
{
    public ConstructPlayerModel characterModel;
    public Descriptor descriptor;

    /// <summary>
    /// Checks the current model descriptor
    /// </summary>
    /// <param name="characterModel">The agent model used for decision making</param>
    /// <param name="descriptor">The  model descriptor required</param>
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