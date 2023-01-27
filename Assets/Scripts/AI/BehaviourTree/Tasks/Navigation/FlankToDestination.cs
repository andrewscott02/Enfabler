using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class FlankToDestination : Node
{
    public AIController agent;
    public CharacterController flankTarget;
    public float distance;
    public bool requiresSameTeam;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="distance">The radius of the roam position, recommend 30</param>
    public FlankToDestination(AIController agent, GameObject flankTarget, float distance, bool requiresSameTeam)
    {
        this.agent = agent;
        this.flankTarget = flankTarget.GetComponent<CharacterController>();
        this.distance = distance;
        this.requiresSameTeam = requiresSameTeam;
    }

    public override NodeState Evaluate()
    {
        if (flankTarget == null) { return NodeState.Failure; }

        if (requiresSameTeam && AIManager.instance.OnSameTeam(agent, flankTarget) == false) { return NodeState.Failure; }

        float flankDistance = Mathf.Clamp(distance, 0, agent.maxDistanceFromModelCharacter);

        Vector3 point = HelperFunctions.GetFlankingPoint(flankTarget.transform.position, agent.currentTarget.transform.position, flankDistance);

        agent.SetDestinationPos(point);
        //Debug.Log("Generated point near target at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }
}